﻿using System.Collections.Generic;
using IdentityServer4.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;
using static IdentityServer4.IdentityServerConstants;
using GovITHub.Auth.Common.Data.Models;

namespace GovITHub.Auth.Common
{
    public class ConfigCommon
    {
        #region configuration keys
        public static readonly string POSTMARK_SERVER_TOKEN = "EmailSender:Postmark:ServerToken";
        public static readonly string EMAIL_FROM_ADDRESS = "EmailSender:Postmark:OriginEmail";

        public static readonly string GOOGLE_CLIENT_ID = "Authentication:Google:GoogleClientId";
        public static readonly string GOOGLE_CLIENT_SECRET = "Authentication:Google:GoogleClientSecret";
        public static readonly string FACEBOOK_APP_ID = "Authentication:Facebook:AppId";
        public static readonly string FACEBOOK_APP_SECRET = "Authentication:Facebook:AppSecret";

        public static readonly string SMTP_ADDRESS = "EmailSender:SMTP:Address";
        public static readonly string SMTP_USERNAME = "EmailSender:SMTP:Username";
        public static readonly string SMTP_PASSWORD = "EmailSender:SMTP:Password";
        public static readonly string SMTP_PORT = "EmailSender:SMTP:Port";
        public static readonly string SMTP_USESSL = "EmailSender:SMTP:UseSSL";

        public static readonly string REDIS_INSTANCE = "Setup:Redis:Instance";
        public static readonly string REDIS_CONFIGURATION = "Setup:Redis:Configuration";

        #endregion

        #region value initializers
        public static readonly string MAIN_ORG_NAME_KEY = "Setup:RootOrganization:Name";
        public static readonly string MAIN_ORG_WEBSITE_KEY = "Setup:RootOrganization:Website";
        public static readonly string MAIN_ORG_ADMIN_USERNAME_KEY = "Setup:RootOrganization:AdminUsername";
        public static readonly string MAIN_ORG_ADMIN_FIRST_PASSWORD_KEY = "Setup:RootOrganization:AdminPassword";
        #endregion

        private readonly IConfigurationRoot configuration;

        public ConfigCommon(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        // scopes define the resources in your system
        internal IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        internal IEnumerable<ApiResource> GetApiResources()
        {
            return configuration.GetSection("Setup:ApiResources").Get<List<ApiResource>>();
        }

        // clients want to access resources (aka scopes)
        internal IEnumerable<Client> GetClients()
        {
            var clients = configuration.GetSection("Setup:Clients").Get<List<Client>>();
            return clients
                .Select(p =>
                {
                    p.ClientSecrets = p.ClientSecrets.Select(y =>
                    {
                        y.Value = y.Value.Sha256();
                        return y;
                    }).ToList();

                    return p;
                });
        }
    }
}