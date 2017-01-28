using System;
using System.Threading.Tasks;

namespace GovITHub.Auth.Common.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
