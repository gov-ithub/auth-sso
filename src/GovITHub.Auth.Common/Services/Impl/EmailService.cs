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
        private IEmailSender emailSender;
        private ApplicationDbContext dbContext;


        /// <summary>
        /// Email service
        /// </summary>
        /// <param name="context">http context accesor</param>
        /// <param name="logger">logger</param>
        /// <param name="dbContext">applicationdbcontext</param>
        public EmailService(IHttpContextAccessor context, ILogger<EmailService> logger, ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            this.context = context;
            this.logger = logger;
            this.dbContext = dbContext;
            this.env = env;

            // configure email sender
            SetEmailSender();
        }

        /// <summary>
        /// Set email sender for organization
        /// </summary>
        protected void SetEmailSender()
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                // grab organization id if authenticated
                var claim = context.HttpContext.User.FindFirst("OrganizationID");
                if (claim != null)
                {
                    long orgId;
                    if(long.TryParse(claim.Value, out orgId))
                    {
                        emailSender = GetEmailSender(orgId);
                    }
                }
            }

            // if email sender not configured, get root settings
            if(emailSender == null)
            {
                emailSender = GetEmailSender(null);
            }
        }

        private IEmailSender GetEmailSender(long? organizationId)
        {
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

            return GetOrganizationEmailSender(org);
        }

        /// <summary>
        /// TODO: replace with Recursive CTE - PostgreSQL, performance
        /// </summary>
        /// <param name="org">Organization</param>
        /// <returns></returns>
        private IEmailSender GetOrganizationEmailSender(Organization org)
        {
            if (org == null)
            {
                throw new ArgumentNullException("organization");
            }
            else
            {
                if (!dbContext.OrganizationSettings.Any(p => p.OrganizationId.Equals(org.Id))) // no settings
                {
                    if(!org.ParentId.HasValue)
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

                //org.OrganizationSetting.EmailSetting.Settings;
                string provider = (from x in dbContext.OrganizationSettings
                                   join y in dbContext.EmailSettings on x.EmailSettingId equals y.Id
                                   join z in dbContext.EmailProviders on y.EmailProviderId equals z.Id
                                   select z.Name).FirstOrDefault();

                switch (provider)
                {
                    case "SMTP":
                        return new SMTPEmailSender(settings, logger, env);
                    case "Postmark":
                        return new PostmarkEmailSender(settings, logger, env);
                    default:
                        throw new NotSupportedException(provider);
                }
            }
        }

       

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return emailSender.SendEmailAsync(email, subject, message);
        }
    }
}