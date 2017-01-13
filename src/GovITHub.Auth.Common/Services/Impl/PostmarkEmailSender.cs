using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostmarkDotNet;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace GovITHub.Auth.Common.Services.Impl
{
    public class PostmarkEmailSender : BaseEmailSender
    {
        public PostmarkEmailSender(string settingsValue, ILogger<EmailService> logger, IHostingEnvironment env) : base(settingsValue, logger, env)
        {
        }

        public override Task SendEmailAsync(string email, string subject, string message)
        {
            var postmarkServerToken = settings.Password;
            var originEmailAddress = settings.FromEmail;

            if (!string.IsNullOrWhiteSpace(postmarkServerToken))
            {
                var emailMessage = new PostmarkMessage()
                {
                    From = originEmailAddress,
                    To = email,
                    Subject = subject,
                    TextBody = message,
                    HtmlBody = message
                };

                var client = new PostmarkClient(postmarkServerToken);
                return client.SendMessageAsync(emailMessage);
            }
            else
            {
                logger.LogError("Postmark server token is not configured, so we're not able to send emails.");
                return Task.FromResult(0);
            }
        }
    }
}
