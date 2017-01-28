using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GovITHub.Auth.Common.Services.Impl
{
    /// <summary>
    /// Base email sender
    /// </summary>
    public abstract class BaseEmailSender : IEmailSender
    {
        public EmailProviderSettings Settings { get; set; }

        protected ILogger<EmailService> Logger { get; set; }
        protected IHostingEnvironment Env { get; set; }

        public abstract Task SendEmailAsync(string email, string subject, string message);

        public BaseEmailSender(EmailProviderSettings settingsValue, ILogger<EmailService> logger, IHostingEnvironment env)
        {
            this.Logger = logger;
            this.Env = env;

            Settings = settingsValue;
        }

        /// <summary>
        /// Build settings
        /// </summary>
        /// <param name="settingsValue">settings</param>
        protected virtual void Build(string settingsValue)
        {
            if (string.IsNullOrEmpty(settingsValue))
            {
                throw new ArgumentNullException("settings");
            }

            Settings = JsonConvert.DeserializeObject<EmailProviderSettings>(settingsValue);

            if (string.IsNullOrEmpty(Settings.Address))
            {
                throw new ArgumentNullException("settings.Address");
            }
        }
    }
}
