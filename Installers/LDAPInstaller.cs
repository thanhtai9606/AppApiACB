using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Options;

namespace App.Installers
{
    public class LDAPInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var ldapSettings = new LDAPSettings();
            configuration.Bind(nameof(ldapSettings), ldapSettings);
            services.AddSingleton(ldapSettings);

        }
    }
}