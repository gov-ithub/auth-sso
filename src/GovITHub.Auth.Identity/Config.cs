// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using IdentityServer4.Services.InMemory;
using System.Collections.Generic;
using System.Security.Claims;

namespace GovITHub.Auth.Identity
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
        #endregion

        // scopes define the resources in your system
        internal static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.OfflineAccess,

                new Scope
                {
                    Name = "api1",
                    DisplayName = "API1 access",
                    Description = "My API"
                }
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
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.OfflineAccess.Name,
                        "api1"
                    }
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
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        "api1"
                    }
                }
            };
        }

        public static List<InMemoryUser> GetUsers()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Subject = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new InMemoryUser
                {
                    Subject = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}