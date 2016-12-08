using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes
{
	public abstract class BaseResourceFileRegexLoader<TRegex> : IDeviceInfoRegexLoader<TRegex> where TRegex : IRegex
	{
		private readonly string _fileName;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger _logger;

		public BaseResourceFileRegexLoader(string fileName, IFileProvider fileProvider, ILogger logger)
		{
			_fileName = fileName;
			_logger = logger;
			_fileProvider = fileProvider;
		}

		public IEnumerable<TRegex> LoadRegularExpressions()
		{
			var fileInfo = _fileProvider.GetFileInfo(_fileName);

			if (fileInfo.Exists)
				return ReadRegularExpressionsFromFile(fileInfo);

			_logger.LogError("Device detector files missing!");

			return Enumerable.Empty<TRegex>();
		}

		public abstract IEnumerable<TRegex> ReadRegularExpressionsFromFile(IFileInfo fileInfo);
	}
}