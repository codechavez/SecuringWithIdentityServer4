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
using Serilog;

namespace CodeChavez.Cerberus
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.Title = "Cerberus | IdentityServer4";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Cerberus | IdentityServer4...starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Cerberus | IdentityServer4...starting web host terminated unexpectedly");
                return -1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
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
