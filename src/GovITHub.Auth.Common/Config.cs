// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;

namespace GovITHub.Auth.Common
{
    public class Config
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

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // clients want to access resources (aka scopes)
        internal static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource Owner Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // OpenID Connect hybrid flow and client credentials client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    RequireConsent = false,

                    ClientSecrets = 
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002" },

                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        "api1",
                        StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true
                },

                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5003" },

                    AllowedScopes = 
                    {
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        "api1"
                    }
                }
            };
        }
    }
}