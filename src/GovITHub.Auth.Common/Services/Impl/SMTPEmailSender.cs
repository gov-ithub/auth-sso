using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GovITHub.Auth.Common.Services.Impl
{
    public class SMTPEmailSender : BaseEmailSender
    {
        public SMTPEmailSender(string settings, ILogger<EmailService> logger, IHostingEnvironment env) : base(settings, logger, env)
        {
        }

        public override Task SendEmailAsync(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = messageBody };

            using (var client = new SmtpClient())
            {
                if (env.IsDevelopment())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                client.Connect(settings.Address, settings.Port, settings.UseSSL);

                if (!string.IsNullOrWhiteSpace(settings.Username) && !string.IsNullOrWhiteSpace(settings.Password))
                {
                    client.Authenticate(settings.Username, settings.Password);
                }

                client.SendAsync(message);
                return client.DisconnectAsync(true);
            }
        }
    }
}
