using GovITHub.Auth.Common.Data.Models;
using GovITHub.Auth.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using GovITHub.Auth.Common.Services.Impl;

namespace GovITHub.Auth.Common.Data
{
    public class ApplicationDataInitializer
    {
        private ApplicationDbContext context;

        public ApplicationDataInitializer(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public async Task InitializeDataAsync(UserManager<ApplicationUser> userManager, IConfigurationRoot configurationService)
        {
            context.Database.Migrate();
            var mainOrg = context.Organizations.FirstOrDefault(t => t.ParentId == null);
            if (mainOrg == null)
            {
                mainOrg = CreateRootOrganization(configurationService);
            }
            string adminUserId = await GetRootOrganizationAdminIdAsync(userManager, configurationService);
            AttachAdminUserToOrganization(mainOrg.Id, adminUserId);

            EnsureSeedData(mainOrg, configurationService);
        }

        private void EnsureSeedData(Organization mainOrg, IConfigurationRoot configurationService)
        {
            EmailProvider smtp, postmark;

            smtp = context.EmailProviders.FirstOrDefault(p => p.Name.Equals("SMTP"));
            if (smtp == null)
            {
                smtp = new EmailProvider()
                {
                    Name = "SMTP"
                };
                context.EmailProviders.Add(smtp);
                context.SaveChanges();
            }

            postmark = context.EmailProviders.FirstOrDefault(p => p.Name.Equals("Postmark"));
            if (postmark == null)
            {
                postmark = new EmailProvider()
                {
                    Name = "Postmark"
                };
                context.EmailProviders.Add(postmark);
                context.SaveChanges();
            }

            if (!context.OrganizationSettings.Any(p => p.OrganizationId.Equals(mainOrg.Id)))
            {
                EmailSetting emailSetting = null;
                if (configurationService.GetSection("EmailSender") != null)
                {
                    var sett = configurationService.GetSection("EmailSender:SMTP").Get<EmailProviderSettings>();
                    if (sett == null || string.IsNullOrEmpty(sett.Address))
                    {
                        sett = configurationService.GetSection("EmailSender:Postmark").Get<EmailProviderSettings>();
                    }

                    if (sett != null && !string.IsNullOrEmpty(sett.Address))
                    {
                        string value = JsonConvert.SerializeObject(sett);
                        emailSetting = new EmailSetting()
                        {
                            EmailProviderId = smtp.Id,
                            Settings = value
                        };
                        context.EmailSettings.Add(emailSetting);
                        context.SaveChanges();
                    }
                }

                if (emailSetting != null)
                {
                    context.OrganizationSettings.Add(new OrganizationSetting()
                    {
                        EmailSettingId = emailSetting.Id,
                        OrganizationId = mainOrg.Id,
                        UseDomainRestriction = false,
                    });
                    context.SaveChanges();
                }
            }

        }

        private Organization CreateRootOrganization(IConfigurationRoot configurationService)
        {
            string orgName = configurationService[Config.MAIN_ORG_NAME_KEY];
            if (string.IsNullOrWhiteSpace(orgName))
                throw new ArgumentException("Root organization name missing from configuration!");
            var mainOrg = new Organization()
            {
                Name = orgName,
                Website = configurationService[Config.MAIN_ORG_WEBSITE_KEY]
            };
            context.Organizations.Add(mainOrg);
            context.SaveChanges();
            return mainOrg;
        }

        /// <summary>
        /// Returns the Id of the user with admin role attached to root organization.
        /// It creates the user if the user is not present, based on configuration.
        /// </summary>
        /// <param name="configurationService">App configuration service</param>
        /// <returns>Id of the admin user, as string.</returns>
        private async Task<string> GetRootOrganizationAdminIdAsync(UserManager<ApplicationUser> userManager, IConfigurationRoot configurationService)
        {
            var defaultAdminUsername = configurationService[Config.MAIN_ORG_ADMIN_USERNAME_KEY];
            if (string.IsNullOrWhiteSpace(defaultAdminUsername))
                throw new ArgumentNullException("Root organization's admin username (email) missing from configuration!");
            var adminUser = await userManager.FindByEmailAsync(defaultAdminUsername);
            if (adminUser == null)
            {
                var defaultAdminPassword = configurationService[Config.MAIN_ORG_ADMIN_FIRST_PASSWORD_KEY];
                if (string.IsNullOrWhiteSpace(defaultAdminPassword))
                    throw new ArgumentNullException("Root organization's admin password missing from configuration!");
                // maybe validate the password strength also ?
                adminUser = new ApplicationUser()
                {
                    Email = defaultAdminUsername,
                    UserName = defaultAdminUsername
                };
                var identityResult = await userManager.CreateAsync(adminUser,
                    defaultAdminPassword);
            }
            return adminUser.Id;
        }

        private void AttachAdminUserToOrganization(long organizationId, string adminUserId)
        {
            var orgUser = context.OrganizationUsers.
                FirstOrDefault(t => t.OrganizationId == organizationId && t.UserId == adminUserId);
            // if not already attached, attach it
            if (orgUser == null)
            {
                orgUser = new OrganizationUser()
                {
                    Level = OrganizationUserLevel.Admin,
                    OrganizationId = organizationId,
                    UserId = adminUserId
                };
                context.OrganizationUsers.Add(orgUser);
                context.SaveChanges();
            }
        }
    }
}
