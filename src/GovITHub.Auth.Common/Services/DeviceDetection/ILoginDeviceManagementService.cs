using System.Threading.Tasks;

namespace GovITHub.Auth.Common.Services.DeviceDetection
{
    public interface ILoginDeviceManagementService
    {
        Task RegisterDeviceLoginAsync(string userId, string userAgentString);
    }
}
