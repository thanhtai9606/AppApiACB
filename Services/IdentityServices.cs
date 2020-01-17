using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using App.Data;
using App.Options;
using App.Domain;
using App.Helpers;

namespace App.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string userName, string email, string password);


        Task<AuthenticationResult> LoginAsync(string userName, string password);

        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        Task<AuthenticationResult> LDAPLoginAsync(string userName, string password);

        AuthenticateResult LdapLogin(string userName, string Password);
        Task<OperationResult> AddPolicy(string userName, string policy);
        Task<OperationResult> CreateRole(string roleName);

        Task<OperationResult> AddToRole(string userName, IEnumerable<string> roleNames);
        Task<OperationResult> RemoveToRole(string userName, IEnumerable<string> roleNames);
        IQueryable<IdentityRole> GetRoles();
        IQueryable<IdentityUser> GetUsers();
        Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName);
        Task<IList<string>> GetRolesAsync(string userName);
        Task<OperationResult> ResetPasswordAsync(string userName, string password);
    }
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly LDAPSettings _ldapSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly AuthDbContext _context;

        public IdentityService(UserManager<IdentityUser> userManager,
                                JwtSettings jwtSettings,
                                TokenValidationParameters tokenValidationParameters,
                                AuthDbContext context,
                                LDAPSettings ldapSettings,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
            _ldapSettings = ldapSettings;
        }

        public async Task<AuthenticationResult> RegisterAsync(string userName, string email, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);

            if (existingUser != null) //???
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this userName address already exists" }
                };
            }

            var newUserId = Guid.NewGuid();
            var newUser = new IdentityUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = userName
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);
            if (!createdUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);//.FindByEmailAsync(userName);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User/password combination is wrong" }
                };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
        public async Task<AuthenticationResult> LDAPLoginAsync(string userName, string password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _ldapSettings.LDAP_Domain, _ldapSettings.Account, _ldapSettings.Password))
            {
                if (context.ValidateCredentials(userName, password))
                {
                    using (var de = new DirectoryEntry(_ldapSettings.LDAP_Path))
                    using (var ds = new DirectorySearcher(de))
                    {

                        var identities = new ClaimsIdentity("custom auth type");
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), "");

                        var user = await _userManager.FindByNameAsync(userName);//.FindByEmailAsync(userName);

                        if (user == null)
                            return await RegisterAsync(userName, userName + "@becamex.com.vn", password);
                        else
                            return await GenerateAuthenticationResultForUserAsync(user);
                    }
                }
                else
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User does not exist" }
                    };
                }
            }

        }

        public AuthenticateResult LdapLogin(string userName, string Password)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _ldapSettings.LDAP_Domain, _ldapSettings.Account, _ldapSettings.Password))
            {
                if (context.ValidateCredentials(userName, Password))
                {
                    using (var de = new DirectoryEntry(_ldapSettings.LDAP_Path))
                    using (var ds = new DirectorySearcher(de))
                    {
                        var identities = new ClaimsIdentity("custom auth type");
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), "");
                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
            return AuthenticateResult.Fail("Invalid user");
        }
        public async Task<OperationResult> AddPolicy(string userName, string policy)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            #region add Claim
            // set claim for User


            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }
            #endregion

            var currentClaim = _userManager.GetClaimsAsync(user);
            // for Claim
            var identity = await _userManager.AddClaimAsync(user, new Claim(policy, "true"));

            if (identity.Succeeded)
            {
                return new OperationResult
                {
                    Success = true,
                    Caption = "Added Success",
                    Message = string.Format("Add Policy {0} for user {1} is complete", policy, userName)

                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Caption = "Failed",
                    //   Message = string.Format("Something wrong when add policy {0} for user {1}", policy, userName)
                    Message = identity.Errors.ToString()

                };
            }

        }

        public async Task<OperationResult> ResetPasswordAsync(string userName, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);

            if (existingUser != null)
            {
                await _userManager.ResetPasswordAsync(existingUser, null, password);
                return new OperationResult
                {
                    Success = true,
                    Caption = "Sucess",
                    Message = string.Format("{0}'s password have been reset")
                };
            }
            return new OperationResult
            {
                Success = false,
                Caption = "Error",
                Message = string.Format("{0}'s password can not been reset")
            };


        }

        public async Task<OperationResult> CreateRole(string roleName)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = roleName
            };
            IdentityResult result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return new OperationResult
                {
                    Success = true,
                    Caption = "Added Success",
                    Message = string.Format("Add Role is complete")

                };
            }
            else
            {
                return new OperationResult
                {
                    Success = false,
                    Caption = "Failed",
                    Message = result.Errors.ToString()

                };
            }
        }

        public async Task<OperationResult> AddToRole(string userName, IEnumerable<string> roleNames)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
            {
                var addRole = await _userManager.AddToRolesAsync(identityUser, roleNames);
                if (!addRole.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Add role failed " + addRole.Errors.ToString(),
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = "Add role Complete ",
                Caption = "Success"
            };
        }
        public async Task<OperationResult> RemoveToRole(string userName, IEnumerable<string> roleNames)
        {
            var identityUser = await _userManager.FindByNameAsync(userName);
            if (identityUser != null)
            {
                var addRole = await _userManager.RemoveFromRolesAsync(identityUser, roleNames);
                if (!addRole.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "Add role failed " + addRole.Errors.ToString(),
                        Caption = "Error"
                    };
                }
            }
            return new OperationResult
            {
                Success = true,
                Message = "Add role Complete ",
                Caption = "Success"
            };
        }

        public IQueryable<IdentityRole> GetRoles()
        {
            return _roleManager.Roles;
        }

        public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public IQueryable<IdentityUser> GetUsers()
        {
            return _userManager.Users;
        }

        public async Task<IList<string>> GetRolesAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            return await _userManager.GetRolesAsync(currentUser);
        }
    }
}