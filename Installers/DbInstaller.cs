
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Data;
using App.Models;
using App.Services;
using BecamexIDC.Pattern.EF.Factory;
using BecamexIDC.Pattern.EF.UnitOfWork;
using BecamexIDC.Pattern.EF.Repositories;
using BecamexIDC.Pattern.EF.DataContext;

namespace App.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options => options.UseMySql(configuration.GetConnectionString("MySqlAuthConnection")));
            services.AddDbContext<ACBSystemContext>(options => options.UseMySql(configuration.GetConnectionString("MySqlConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddRoles<IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<AuthDbContext>();
             services.AddScoped<IDataContextAsync, ACBSystemContext>();

             services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();

             #region  Repository Generic
            
            services.AddScoped<IRepositoryAsync<Product>, Repository<Product>>();    
            services.AddScoped<IRepositoryAsync<Customer>, Repository<Customer>>();  
            services.AddScoped<IRepositoryAsync<SaleHeader>, Repository<SaleHeader>>();  
            services.AddScoped<IRepositoryAsync<SaleDetail>, Repository<SaleDetail>>();  

            services.AddScoped<ICustomerService, CustomerService>();    
            services.AddScoped<IProductService, ProductService>();    
            services.AddScoped<ISaleHeaderService, SaleHeaderService>();
            services.AddScoped<ISaleDetailService, SaleDetailService>();    
            #endregion
           
        }
    }
}

