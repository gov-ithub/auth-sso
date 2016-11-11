using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostmarkDotNet;
using System.Threading.Tasks;

namespace GovITHub.Auth.Identity.Services.Impl
{
    public class PostmarkEmailSender : IEmailSender
    {
        IConfigurationRoot configurationRootService;
        private readonly ILogger<PostmarkEmailSender> logger;
        public PostmarkEmailSender(IConfigurationRoot configurationRootService, ILogger<PostmarkEmailSender> logger)
        {
            this.configurationRootService = configurationRootService;
            this.logger = logger;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var postmarkServerToken = configurationRootService[Config.POSTMARK_SERVER_TOKEN];
            var originEmailAddress = configurationRootService[Config.EMAIL_FROM_ADDRESS];
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
                logger.LogWarning("Postmark server token is not configured, so we're not able to send emails.");
                return Task.FromResult(0);
            }
        }
    }
}
