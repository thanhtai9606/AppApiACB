using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Installers
{
    public class PolicyInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
           services.AddAuthorization(options =>
           {
               options.AddPolicy("TagViewer", builder =>
               {

                   builder.RequireClaim("tags.view", "true");
               });             
             options.AddPolicy("StudentGet", builder =>
               {
                   builder.RequireClaim("student.get", "true");
                   
               });
            options.AddPolicy("StudentAdd", builder =>
               {
                   builder.RequireClaim("student.add", "true");
                   
               });

           });
        }
    }
}