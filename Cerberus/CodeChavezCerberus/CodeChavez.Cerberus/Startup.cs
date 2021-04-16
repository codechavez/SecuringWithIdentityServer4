using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CodeChavez.Cerberus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();
        }

        public IConfiguration Configuration { get; }
        internal AppConfigurations AppConfigs { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppConfigs = Configuration.GetSection("AppConfigs").Get<AppConfigurations>();
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            #region IdentityServer 4 Configurations & Registrations

            services.AddControllers();

            services.AddIdentityServer(options=> 
            {
                options.Discovery.CustomEntries.Add("client_registration", "~/connect/register");
            })
                .AddDeveloperSigningCredential()
                // Configures Clients and Resources
                .AddConfigurationStore(ops =>
                {  
                    ops.DefaultSchema = AppConfigs.DbOptions.Schema;
                    ops.ConfigureDbContext = dbc =>
                        dbc.UseSqlServer(AppConfigs.DbOptions.ConnectionString,
                        sql =>
                        {
                            sql.MigrationsAssembly(migrationsAssembly);
                            sql.MigrationsHistoryTable("__EFMigrationsHistory", AppConfigs.DbOptions.Schema);
                        });
                })
                // Configures tokens, consents and codes, etc.
                .AddOperationalStore(ops =>
                {
                    ops.DefaultSchema = AppConfigs.DbOptions.Schema;
                    ops.ConfigureDbContext = odbc =>
                    odbc.UseSqlServer(AppConfigs.DbOptions.ConnectionString,
                    sql =>
                    {
                        sql.MigrationsAssembly(migrationsAssembly);
                        sql.MigrationsHistoryTable("__EFMigrationsHistory", AppConfigs.DbOptions.Schema);
                    });

                    ops.EnableTokenCleanup = AppConfigs.DbOptions.EnableTokenCleanup;
                    ops.TokenCleanupInterval = AppConfigs.DbOptions.TokenCleanupInterval;
                    ops.TokenCleanupBatchSize = AppConfigs.DbOptions.TokenCleanupBatchSize;
                });
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var grantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            grantContext.Database.Migrate();

            var configContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            configContext.Database.Migrate();
        }
    }
}
