using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Data.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace GovITHub.Auth.Common.Services.Impl
{
    /// <summary>
    /// Email Service "Factory" responsible with providing the implementation for a specific OrganizationID
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IHttpContextAccessor context;
        private readonly ILogger<EmailService> logger;
        private readonly IHostingEnvironment env;
        private readonly IDistributedCache cache;
        private IEmailSender emailSender;
        private ApplicationDbContext dbContext;


        /// <summary>
        /// Email service
        /// </summary>
        /// <param name="context">http context accesor</param>
        /// <param name="logger">logger</param>
        /// <param name="dbContext">applicationdbcontext</param>
        public EmailService(IHttpContextAccessor context, ILogger<EmailService> logger, ApplicationDbContext dbContext, IHostingEnvironment env, IDistributedCache cache)
        {
            this.context = context;
            this.logger = logger;
            this.dbContext = dbContext;
            this.env = env;
            this.cache = cache;
        }

        /// <summary>
        /// Set email sender for organization
        /// </summary>
        private void SetEmailSender(long? organizationId)
        {
            // if email sender not configured, get root settings
            if (emailSender == null)
            {
                emailSender = GetEmailSender(organizationId);
            }
        }

        /// <summary>
        /// Retrieve email sender for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private BaseEmailSender GetEmailSender(long? organizationId)
        {
            // get cached email sernder
            BaseEmailSender sender = GetCachedEmailSender(organizationId);

            if (sender != null)
            {
                return sender;
            }

            Organization org = null;
            if (!organizationId.HasValue || organizationId <= 0)
            {
                org = dbContext.Organizations.FirstOrDefault(p => !p.ParentId.HasValue);

            }
            else
            {
                org = dbContext.Organizations.Find(organizationId);
            }

            if (org == null)
            {
                throw new ArgumentNullException("organization", string.Format("Organization {0} not found", organizationId));
            }

            sender = GetOrganizationEmailSender(org);

            SetCachedEmailSender(sender, org);

            return sender;
        }

        private void SetCachedEmailSender(BaseEmailSender sender, Organization org)
        {
            if(!org.ParentId.HasValue)
            {
                cache.SetString(CacheKeys.EmailSettingsRoot, sender.Settings.ToString());
            }
            else
            {
                cache.SetString(string.Format(CacheKeys.EmailSettingsOrg, org.Id), sender.Settings.ToString());
            }
        }

        private BaseEmailSender GetCachedEmailSender(long? organizationId)
        {
            string settings = string.Empty;
            if (!organizationId.HasValue || organizationId <= 0)
            {
                settings = cache.GetString(CacheKeys.EmailSettingsRoot);
            }
            else
            {
                settings = cache.GetString(string.Format(CacheKeys.EmailSettingsOrg, organizationId.Value));
            }

            if (!string.IsNullOrEmpty(settings))
            {
                return BuildEmailSender(BuildProivderSettings(settings));
            }

            return null;
        }

        /// <summary>
        /// TODO: replace with Recursive CTE - PostgreSQL, performance
        /// </summary>
        /// <param name="org">Organization</param>
        /// <returns></returns>
        private BaseEmailSender GetOrganizationEmailSender(Organization org)
        {
            if (org == null)
            {
                throw new ArgumentNullException("organization");
            }
            else
            {
                if (!dbContext.OrganizationSettings.Any(p => p.OrganizationId.Equals(org.Id))) // no settings
                {
                    if (!org.ParentId.HasValue)
                    {
                        throw new Exception("Root organization settings not initialized");
                    }
                    else
                    {
                        return GetEmailSender(org.ParentId);
                    }
                }

                string settings = (from x in dbContext.OrganizationSettings
                                   join y in dbContext.EmailSettings on x.EmailSettingId equals y.Id
                                   select y.Settings).FirstOrDefault();

                return BuildEmailSender(settings);

            }
        }

        private BaseEmailSender BuildEmailSender(string settings)
        {
            return BuildEmailSender(BuildProivderSettings(settings));
        }

        private BaseEmailSender BuildEmailSender(EmailProviderSettings providerSettings)
        {
            switch (providerSettings.ProviderName)
            {
                case "SMTP":
                    return new SMTPEmailSender(providerSettings, logger, env);
                case "Postmark":
                    return new PostmarkEmailSender(providerSettings, logger, env);
                default:
                    throw new NotSupportedException(providerSettings.ProviderName);
            }
        }

        protected EmailProviderSettings BuildProivderSettings(string settingsValue)
        {
            if (string.IsNullOrEmpty(settingsValue))
            {
                throw new ArgumentNullException("settings");
            }

            var settings = JsonConvert.DeserializeObject<EmailProviderSettings>(settingsValue);


            if (string.IsNullOrEmpty(settings.Address))
            {
                throw new ArgumentNullException("settings.Address");
            }

            return settings;
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            long? orgId = null;
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // grab organization id if authenticated
                var claim = context.HttpContext.User.FindFirst("OrganizationID");
                if (claim != null)
                {
                    long organizationId;
                    if(long.TryParse(claim.Value, out organizationId))
                    {
                        orgId = organizationId;
                    }
                }
            }

            return SendEmailAsync(orgId, email, subject, message);
        }

        public Task SendEmailAsync(long? organizationId, string email, string subject, string message)
        {
            SetEmailSender(organizationId);

            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}