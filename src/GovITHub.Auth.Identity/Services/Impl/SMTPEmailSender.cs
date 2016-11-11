using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Threading.Tasks;
using System;

namespace GovITHub.Auth.Identity.Services.Impl
{
    public class SMTPEmailSender : IEmailSender
    {
        #region private fields
        IConfigurationRoot configurationRootService;
        private readonly ILogger<SMTPEmailSender> logger;
        IHostingEnvironment env;

        private string smtpAddress, smtpUsername, smtpPassword;
        int smtpPort;
        bool useSSL;
        #endregion

        public SMTPEmailSender(IHostingEnvironment env, IConfigurationRoot configurationRootService, ILogger<SMTPEmailSender> logger)
        {
            this.env = env;
            this.configurationRootService = configurationRootService;
            this.logger = logger;
            ReadConfiguration();
        }

        private void ReadConfiguration()
        {
            smtpAddress = configurationRootService[Config.SMTP_ADDRESS];
            smtpUsername = configurationRootService[Config.SMTP_USERNAME];
            smtpPassword = configurationRootService[Config.SMTP_PASSWORD];
            smtpPort = !string.IsNullOrWhiteSpace(configurationRootService[Config.SMTP_PORT]) ?
                Int32.Parse(configurationRootService[Config.SMTP_PORT]) : 25; // default SMTP port, if not specified
            useSSL = !string.IsNullOrWhiteSpace(configurationRootService[Config.SMTP_USESSL]) ?
                bool.Parse(configurationRootService[Config.SMTP_USESSL]) : false;
        }

        public Task SendEmailAsync(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Joey Tribbiani", "joey@friends.com"));
            message.To.Add(new MailboxAddress("Mrs. Chanandler Bong", email));
            message.Subject = subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = messageBody };

            using (var client = new SmtpClient())
            {
                if (env.IsDevelopment())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }
                client.Connect(smtpAddress, smtpPort, useSSL);

                if (!string.IsNullOrWhiteSpace(smtpUsername) && !string.IsNullOrWhiteSpace(smtpPassword))
                {
                    client.Authenticate(smtpUsername, smtpPassword);
                }

                client.SendAsync(message);
                return client.DisconnectAsync(true);
            }
        }
    }
}
