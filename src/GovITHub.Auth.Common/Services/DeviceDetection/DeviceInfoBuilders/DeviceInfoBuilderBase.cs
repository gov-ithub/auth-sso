using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders
{
	public abstract class DeviceInfoBuilderBase<TRegex> where TRegex : IRegex
    {
        public DeviceInfoBuilderBase(Regexes.IDeviceInfoRegexLoader<TRegex> regexLoader)
        {
			Regexes = new Lazy<IEnumerable<TRegex>>(() => regexLoader.LoadRegularExpressions(), true);
        }

        public Lazy<IEnumerable<TRegex>> Regexes { get; private set; }

        protected void BuildInternal(DeviceInfo deviceInfo, string userAgent, Action<DeviceInfo, string> propertySetter)
        {
            Debug.Assert(deviceInfo != null);
            Debug.Assert(!String.IsNullOrEmpty(userAgent));
            Debug.Assert(propertySetter != null);

            var matchingItem = Regexes.Value.Select(r => new
            {
                Match = Regex.Match(userAgent, r.Regex),
                Regex = r
            })
           .FirstOrDefault(tuple => tuple.Match.Success);
            if (matchingItem != null)
            {
                var regex = matchingItem.Regex;
                var match = matchingItem.Match;
                string value = BuildDeviceInfo(regex, match, userAgent);
                propertySetter(deviceInfo, value);
            }
        }

        protected virtual string BuildDeviceInfo(TRegex regex, Match match, string userAgent)
        {
            var version = IsFixedVersion(regex) ? regex.Version : match.GetCapturingGroupValue(regex.Version);
            var value = String.IsNullOrEmpty(version) ? regex.Name : $"{regex.Name} {version}";
            return value;
        }

        protected bool IsFixedVersion(TRegex regex)
        {
            return String.IsNullOrEmpty(regex.Version) || !regex.Version.StartsWith("$");
        }
    }
}
