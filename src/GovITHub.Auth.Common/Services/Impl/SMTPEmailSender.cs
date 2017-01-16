﻿using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace GovITHub.Auth.Common.Services.Impl
{
    public class SMTPEmailSender : BaseEmailSender
    {
        public SMTPEmailSender(EmailProviderSettings settings, ILogger<EmailService> logger, IHostingEnvironment env)
            : base(settings, logger, env)
        {
        }

        public override async Task SendEmailAsync(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Settings.FromName, Settings.FromEmail));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = messageBody };

            using (var client = new SmtpClient())
            {
                if (Env.IsDevelopment())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                await client.ConnectAsync(Settings.Address, Settings.Port, Settings.UseSSL);

                if (!string.IsNullOrWhiteSpace(Settings.Username) && !string.IsNullOrWhiteSpace(Settings.Password))
                {
                    await client.AuthenticateAsync(Settings.Username, Settings.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
