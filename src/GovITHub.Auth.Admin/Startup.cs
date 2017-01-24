using GovITHub.Auth.Admin.Services;
using GovITHub.Auth.Admin.Services.Impl;
using GovITHub.Auth.Common;
using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Data.Impl;
using GovITHub.Auth.Common.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySQL.Data.Entity.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Threading.Tasks;

namespace GovITHub.Auth.Admin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("connectionstrings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string mySqlConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(ApplicationUser).GetTypeInfo().Assembly.GetName().Name;

            services.
                AddEntityFramework().
                AddDbContext<ApplicationDbContext>(options => options.UseMySQL(mySqlConnectionString));

            services.AddMvc();

            services.AddSingleton<ISampleRepository, SampleRepository>();
            services.AddTransient<IOrganizationRepository, OrganizationRepository>();
            services.AddTransient<Common.Data.Contract.IOrganizationUserRepository, OrganizationUserRepository>();
            services.AddTransient<IUserClaimsExtender, UserClaimsExtender>();

            // Add auth common services
            services.AddAuthCommonServices(Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, IUserClaimsExtender userClaimsExtender)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });

            var openIdConnectOptions = Configuration.GetSection("Authentication:OpenIdConnectOptions").Get<OpenIdConnectOptions>();
            if (openIdConnectOptions == null || string.IsNullOrEmpty(openIdConnectOptions.Authority))
            {
                throw new Exception("Missing open id connect options from config file");
            }

            openIdConnectOptions.Events = new OpenIdConnectEvents()
            {
                OnTicketReceived = (context) =>
                {
                    context.Principal = userClaimsExtender.TransformClaims(context.Ticket.Principal);
                    return Task.CompletedTask;
                }
            };

            app.UseOpenIdConnectAuthentication(openIdConnectOptions);

            //app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
