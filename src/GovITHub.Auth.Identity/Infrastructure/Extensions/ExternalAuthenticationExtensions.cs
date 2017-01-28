using AspNet.Security.OAuth.LinkedIn;
using GovITHub.Auth.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace GovITHub.Auth.Identity.Infrastructure.Extensions
{
    public static class ExternalAuthenticationExtensions
    {
        public static void AddFacebookAuthentication(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            string facebookAppId = configuration[Config.FACEBOOK_APP_ID];
            string facebookAppSecret = configuration[Config.FACEBOOK_APP_SECRET];
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

        public static void AddGoogleAuthentication(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            string googleClientId = configuration[Config.GOOGLE_CLIENT_ID];
            string googleClientSecret = configuration[Config.GOOGLE_CLIENT_SECRET];
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
