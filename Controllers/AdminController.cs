using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Domain;
using App.Helpers;
using App.Routes;
using App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace App.Controllers
{

   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : Controller
    {
        private readonly IIdentityService _identityService;

        public AdminController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

       
        [HttpPost(ApiRoutes.Identity.AddPolicy)]
        public async Task<IActionResult> AddPolicy([FromBody] string Username, string Policy)
        {
            var operationResult = await _identityService.AddPolicy(Username, Policy);

            return Ok(operationResult);
        }

        [HttpPost(ApiRoutes.Identity.CreateRole)]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
            return Ok(await _identityService.CreateRole(roleName));
        }

     

        [HttpGet(ApiRoutes.Identity.GetUsers)]
        public IActionResult GetUsers()
        {            
            return Ok( _identityService.GetUsers());
        }

        [HttpPost(ApiRoutes.Identity.AddToRole)]
        public async Task<IActionResult> AddToRole([FromBody] UserToRole userToRole)
        {
            return Ok(await _identityService.AddToRole(userToRole.Username, userToRole.roles));
        }
        [HttpDelete(ApiRoutes.Identity.RemoveToRole)]
        public async Task<IActionResult> RemoveToRole(UserToRole userToRole)
        {            
            return Ok(await _identityService.RemoveToRole(userToRole.Username, userToRole.roles));
        }

        
        [HttpGet(ApiRoutes.Identity.GetRoles)]
        public IActionResult GetRoles()
        {            
            return Ok( _identityService.GetRoles());
        }

        
        [HttpGet(ApiRoutes.Identity.GetUsersInRoleAsync)]
        public IActionResult GetUsersInRoleAsync(string roleName)
        {            
            return Ok( _identityService.GetUsersInRoleAsync(roleName).Result);
        }

        [HttpGet(ApiRoutes.Identity.GetRolesAsync)]
        public IActionResult GetRolesAsync(string userName)
        {            
            return Ok( _identityService.GetRolesAsync(userName).Result);
        }



        [HttpPut(ApiRoutes.Identity.ResetPasswordAsync)]
        public async Task<IActionResult> ResetPasswordAsync(string userName, string Password)
        {            
            return Ok( await _identityService.ResetPasswordAsync(userName, Password));
        }
    }
}