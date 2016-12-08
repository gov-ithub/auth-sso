using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes
{
	public class SimpleResourceFileRegexLoader<TRegex> : BaseResourceFileRegexLoader<TRegex> where TRegex : IRegex
	{
		public SimpleResourceFileRegexLoader(string fileName, IFileProvider fileProvider, ILoggerFactory loggerFactory)
			: base(fileName, fileProvider, loggerFactory.CreateLogger<SimpleResourceFileRegexLoader<TRegex>>())
		{ }

		public override IEnumerable<TRegex> ReadRegularExpressionsFromFile(IFileInfo fileInfo)
		{
			var serializer = new DeserializerBuilder()
			   .WithNamingConvention(namingConvention: new CamelCaseNamingConvention())
			   .IgnoreUnmatchedProperties()
			   .Build();
			//var assembly = typeof(DeviceInfoBuilderBase<>).GetTypeInfo().Assembly;
			using (var stream = fileInfo.CreateReadStream())
			using (var reader = new StreamReader(stream))
			{
				var collection = serializer.Deserialize<List<TRegex>>(reader);
				return collection;
			}
		}
	}
}
