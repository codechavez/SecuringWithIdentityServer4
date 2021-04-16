using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChavez.Cerberus
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Cerberus | IdentityServer4";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseHttpSys(options =>
                    {
                        options.Authentication.Schemes = AuthenticationSchemes.Negotiate;
                        options.Authentication.AllowAnonymous = true;
                        options.UrlPrefixes.Add("http://+:44366/cerberus");
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
