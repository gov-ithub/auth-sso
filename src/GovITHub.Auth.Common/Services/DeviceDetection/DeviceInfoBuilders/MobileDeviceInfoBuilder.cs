using System;
using System.Linq;
using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using System.Text.RegularExpressions;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes;

namespace GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders
{
    public class MobileDeviceInfoBuilder : DeviceInfoBuilderBase<MobileDeviceRegex>, IDeviceInfoBuilder
    {
        public MobileDeviceInfoBuilder(IDeviceInfoRegexLoader<MobileDeviceRegex> regexLoader)
            : base(regexLoader)
        {
        }

        public void Build(DeviceInfo deviceInfo, string userAgent)
        {
            BuildInternal(deviceInfo, userAgent, (di, value) =>
            {
                di.MobileDevice = value;
            });
        }

        protected override string BuildDeviceInfo(MobileDeviceRegex regex, Match match, string userAgent)
        {
            if (!String.IsNullOrEmpty(regex.Model))
            {
                return regex.Model;
            }

            var model = regex.Models.Select(m => new
            {
                Match = Regex.Match(userAgent, m.Regex),
                Model = m
            })
            .FirstOrDefault(m => m.Match.Success);

            if (model == null)
            {
                return regex.Name;
            }

            var deviceModelName = GetDeviceModelName(model.Model, model.Match);
            return $"{regex.Name} {deviceModelName}";
        }

        private static string GetDeviceModelName(DeviceModel deviceModel, Match deviceModelMatch)
        {
            if (deviceModel.Model.Contains("$"))
            {
                var capturingGroups = Regex.Matches(deviceModel.Model, @"\$\d")
                    .Cast<Match>()
                    .Select(m => m.Value)
                    .Aggregate(deviceModel.Model, (modelName, capturingGroup) =>
                    {
                        return modelName.Replace(capturingGroup, deviceModelMatch.GetCapturingGroupValue(capturingGroup));
                    });
                return capturingGroups;
            }

            return deviceModel.Model;
        }
    }
}
