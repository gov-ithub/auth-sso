using AspNet.Security.OAuth.LinkedIn;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;

namespace GovITHub.Auth.Identity.Infrastructure.Extensions
{
    /// <summary>
    /// This class provides helper methods for enabling external authentication providers.
    /// </summary>
    public static class ExternalAuthenticationExtensions
    {
        /// <summary>
        /// Add Facebook authentication to the application.
        /// </summary>
        /// <param name="app">Current application request pipeline.</param>
        /// <param name="configuration">The configuration object from which to get <c>AppId</c> and <c>AppSecret</c>.</param>
        public static void AddFacebookAuthentication(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            string facebookAppId = configuration["Authentication:Facebook:AppId"];
            string facebookAppSecret = configuration["Authentication:Facebook:AppSecret"];
            var facebookOptions = new FacebookOptions
            {
                AppId = facebookAppId,
                AppSecret = facebookAppSecret
            };
            if (facebookOptions.IsConfigurationValid())
            {
                app.UseFacebookAuthentication(facebookOptions);
            }
        }

        /// <summary>
        /// Add Google authentication to the application.
        /// </summary>
        /// <param name="app">Current application request pipeline.</param>
        /// <param name="configuration">The configuration object from which to get <c>ClientId</c> and <c>ClientSecret</c>.</param>
        public static void AddGoogleAuthentication(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            string googleClientId = configuration["Authentication:Google:GoogleClientId"];
            string googleClientSecret = configuration["Authentication:Google:GoogleClientSecret"];
            var googleOptions = new GoogleOptions
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret
            };
            if (googleOptions.IsConfigurationValid())
            {
                app.UseGoogleAuthentication(googleOptions);
            }
        }

        /// <summary>
        /// Add LinkedIn authentication to the application.
        /// </summary>
        /// <param name="app">Current application request pipeline.</param>
        /// <param name="configuration">The configuration object from which to get <c>ClientId</c> and <c>ClientSecret</c>.</param>
        public static void AddLinkedInAuthentication(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            var linkedInAuthenticationOptions = new LinkedInAuthenticationOptions
            {
                ClientId = configuration["Authentication:LinkedIn:ClientId"],
                ClientSecret = configuration["Authentication:LinkedIn:ClientSecret"]
            };
            if (linkedInAuthenticationOptions.IsConfigurationValid())
            {
                app.UseLinkedInAuthentication(linkedInAuthenticationOptions);
            }
        }

        private static bool IsConfigurationValid(this OAuthOptions options)
        {
            return !String.IsNullOrEmpty(options.ClientId) && !String.IsNullOrEmpty(options.ClientSecret);
        }
    }
}
