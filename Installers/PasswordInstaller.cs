using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Options;

namespace App.Installers
{
    public class PasswordInstalller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var appIdentitySettingsSection = configuration.GetSection("AppIdentitySettings");
            services.Configure<AppIdentitySettings>(appIdentitySettingsSection);

            // var appSettings = appIdentitySettingsSection.Get<AppSettings>();
            var passwordSettings = appIdentitySettingsSection.Get<PasswordSettings>();
            var lockoutSettings = appIdentitySettingsSection.Get<LockoutSettings>();
            var userSettings = appIdentitySettingsSection.Get<UserSettings>();

            services.Configure<IdentityOptions>(options =>
                      {
                          // Password settings
                          options.Password.RequireDigit = passwordSettings.RequireDigit;
                          options.Password.RequiredLength = passwordSettings.RequiredLength;
                          options.Password.RequireNonAlphanumeric = passwordSettings.RequireNonAlphanumeric;
                          options.Password.RequireUppercase = passwordSettings.RequireUppercase;
                          options.Password.RequireLowercase = passwordSettings.RequireLowercase;
                          options.Password.RequiredUniqueChars = passwordSettings.RequiredUniqueChars;

                          // Lockout settings
                          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutSettings.DefaultLockoutTimeSpanInMins);
                          options.Lockout.MaxFailedAccessAttempts = lockoutSettings.MaxFailedAccessAttempts;
                          options.Lockout.AllowedForNewUsers = lockoutSettings.AllowedForNewUsers;

                          // User settings
                          options.User.RequireUniqueEmail = userSettings.RequireUniqueEmail;
                      });
        }
    }
}