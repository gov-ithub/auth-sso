using GovITHub.Auth.Identity.Data;
using GovITHub.Auth.Identity.Infrastructure.Configuration;
using GovITHub.Auth.Identity.Models;
using GovITHub.Auth.Identity.Services;
using GovITHub.Auth.Identity.Services.Impl;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQL.Data.Entity.Extensions;
using System;
using System.Reflection;

namespace GovITHub.Auth.Identity
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("connectionstrings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string mySqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            // Add framework services.
            services.
                AddEntityFramework().
                AddDbContext<ApplicationDbContext>(options => options.UseMySQL(mySqlConnectionString));

            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureAudit();
            services.ConfigureLocalization(mySqlConnectionString, migrationsAssembly);

            services.AddMvc(options =>
            {
                options.ConfigureAudit();
            });

            // Add application services.
            services.AddTransient<ConfigurationDataInitializer>();
            services.AddTransient<ApplicationDataInitializer>();
            services.AddTransient<IEmailSender, PostmarkEmailSender>();
            services.AddTransient<IEmailSender, SMTPEmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddSingleton(Configuration);

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // Adds IdentityServer
            services.AddDeveloperIdentityServer()
                .AddConfigurationStore(builder =>
                    builder.UseMySQL(mySqlConnectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder =>
                    builder.UseMySQL(mySqlConnectionString, options =>
                        options.MigrationsAssembly(migrationsAssembly)))
                .AddAspNetIdentity<ApplicationUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, ConfigurationDataInitializer cfgDataInitializer,
            ApplicationDataInitializer appDataInitializer)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            var logger = loggerFactory.CreateLogger<Startup>();

            app.UseCors("CorsPolicy");

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Adds IdentityServer
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,

                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            InitGoogleAuthentication(app, logger);
            InitFacebookAuthentication(app, logger);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(name: "signin-google",
                     template: "signin-google", defaults: new { controller = "Account", action = "ExternalLoginCallback" });
                routes.MapRoute(
                    name : "DefaultApi",
                    template : "api/{controller}/{id?}"
                    );
            });

            try
            {
                appDataInitializer.InitializeData();
                cfgDataInitializer.InitializeData();
            }
            catch (Exception ex)
            {
                logger.LogCritical("Error initializing database. Reason : {0}", ex);
            }
        }

        private void InitFacebookAuthentication(IApplicationBuilder app, ILogger<Startup> logger)
        {
            string facebookAppId = Configuration[Config.FACEBOOK_APP_ID];
            string facebookAppSecret = Configuration[Config.FACEBOOK_APP_SECRET];
            if (!string.IsNullOrWhiteSpace(facebookAppId) &&
                !string.IsNullOrWhiteSpace(facebookAppSecret))
            {

                app.UseFacebookAuthentication(new FacebookOptions
                {
                    AppId = facebookAppId,
                    AppSecret = facebookAppSecret
                });
            }
        }

        private void InitGoogleAuthentication(IApplicationBuilder app, ILogger logger)
        {
            string googleClientId = Configuration[Config.GOOGLE_CLIENT_ID];
            string googleClientSecret = Configuration[Config.GOOGLE_CLIENT_SECRET];
            if (!string.IsNullOrWhiteSpace(googleClientId) &&
                !string.IsNullOrWhiteSpace(googleClientSecret))
            {
                var googleOptions = new GoogleOptions
                {
                    ClientId = googleClientId,
                    ClientSecret = googleClientSecret
                };
                app.UseGoogleAuthentication(googleOptions);
            }
            else
            {
                logger.LogWarning("Google external authentication credentials not set.");
            }
        }
    }
}
