using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes;

namespace GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders
{
    public class BrowserInfoBuilder : DeviceInfoBuilderBase<BrowserRegex>, IDeviceInfoBuilder
    {
        public BrowserInfoBuilder(IDeviceInfoRegexLoader<BrowserRegex> regexLoader) : base(regexLoader)
        {
        }

        public void Build(DeviceInfo deviceInfo, string userAgent)
        {
            BuildInternal(deviceInfo, userAgent, (info, value) =>
            {
                deviceInfo.Browser = value;
            });
        }
    }
}
