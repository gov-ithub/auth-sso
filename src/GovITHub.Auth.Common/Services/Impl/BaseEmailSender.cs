﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovITHub.Auth.Common.Services.Impl
{
    /// <summary>
    /// Base email sender
    /// </summary>
    public abstract class BaseEmailSender : IEmailSender
    {
        public EmailProviderSettings Settings { get; set; }

        protected readonly ILogger<EmailService> logger;
        protected readonly IHostingEnvironment env;

        public abstract Task SendEmailAsync(string email, string subject, string message);

        public BaseEmailSender(EmailProviderSettings settingsValue, ILogger<EmailService> logger, IHostingEnvironment env)
        {
            this.logger = logger;
            this.env = env;

            Settings = settingsValue;
        }

        /// <summary>
        /// Build settings
        /// </summary>
        /// <param name="settings"></param>
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