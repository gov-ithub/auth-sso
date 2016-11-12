using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using MySQL.Data.Entity.Extensions;
using System.Collections.Generic;
using System.Globalization;
using WebApiContrib.Core.Formatter.Csv;

namespace GovITHub.Auth.Identity.Infrastructure.Configuration
{
    public static class LocalizationConfiguration
    {
        public static IServiceCollection ConfigureLocalization(this IServiceCollection services, 
            string connectionString, string migrationsAssembly)
        {
            services.AddDbContext<LocalizationModelContext>(options => 
                options.UseMySQL(connectionString , mo => mo.MigrationsAssembly(migrationsAssembly)));
            // Requires that LocalizationModelContext is defined
            services.AddSqlLocalization(options => options.UseTypeFullNames = true);

            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo> {
                        new CultureInfo("ro-RO"),
                        new CultureInfo("en-US")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "ro-RO", uiCulture: "ro-RO");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }
            );

            var csvFormatterOptions = new CsvFormatterOptions();

            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new CsvInputFormatter(csvFormatterOptions));
                options.OutputFormatters.Add(new CsvOutputFormatter(csvFormatterOptions));
                options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
            });

            // services.AddScoped<ValidateMimeMultipartContentFilter>();
            return services;
        }
    }
}
