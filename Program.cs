using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            ChangePort();
        }
        /// <summary>
        /// Change Port
        /// But Problem with https
        /// If use https problem with connectionString
        /// </summary>

        static void ChangePort()
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
            var Url = config.GetValue<string>("Host");// Set port 
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls(Url)
                .Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
