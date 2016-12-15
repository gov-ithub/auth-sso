using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.YamlSchema;
using Moq;
using System;
using Xunit;

namespace GovITHub.Auth.Common.Tests.Services.DeviceDetection.DeviceInfoBuilders
{
    public class BrowserInfoBuilderTests : System.IDisposable
    {
        private readonly Mock<Microsoft.Extensions.FileProviders.IFileProvider> fileProviderMock;
        private readonly Mock<Microsoft.Extensions.FileProviders.IFileInfo> fileInfoMock;
        private readonly Mock<Microsoft.Extensions.Logging.ILoggerFactory> loggerFactoryMock;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger> loggerMock;

        private readonly BrowserInfoBuilder _builder;

        public BrowserInfoBuilderTests()
        {
            fileProviderMock = new Mock<Microsoft.Extensions.FileProviders.IFileProvider>(MockBehavior.Strict);
            fileInfoMock = new Mock<Microsoft.Extensions.FileProviders.IFileInfo>(MockBehavior.Strict);
            loggerFactoryMock = new Mock<Microsoft.Extensions.Logging.ILoggerFactory>(MockBehavior.Strict);
            loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger>(MockBehavior.Strict);

            loggerFactoryMock.Setup(x => x.CreateLogger("GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes.SimpleResourceFileRegexLoader")).Returns(loggerMock.Object);

            _builder = new BrowserInfoBuilder(new SimpleResourceFileRegexLoader<BrowserRegex>("GovITHub.Auth.Common.Tests.browsers.yml", fileProviderMock.Object, loggerFactoryMock.Object));
        }

        [Fact]
        public void Build_WhenFileDoesNotExist_ThenErrorIsLogged()
        {
            fileProviderMock.Setup(x => x.GetFileInfo("GovITHub.Auth.Common.Tests.browsers.yml")).Returns(fileInfoMock.Object);

            fileInfoMock.SetupGet(x => x.Exists).Returns(false);

            loggerMock
                .Setup(x => x.Log(It.IsAny<Microsoft.Extensions.Logging.LogLevel>(), It.IsAny<Microsoft.Extensions.Logging.EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Callback<Microsoft.Extensions.Logging.LogLevel, Microsoft.Extensions.Logging.EventId, object, Exception, Func<object, Exception, string>>((logLevelCallback, eventIdCallback, objectCallback, exceptionCallback, formatterCallback) =>
                {
                    Assert.Equal(Microsoft.Extensions.Logging.LogLevel.Error, logLevelCallback);
                    Assert.Equal("Device detector files missing!", formatterCallback(objectCallback, exceptionCallback));
                });

            DeviceInfo deviceInfo = new DeviceInfo
            {
                UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36 OPR/41.0.2353.56"
            };

            _builder.Build(deviceInfo, deviceInfo.UserAgent);

            Assert.Equal(null, deviceInfo.Browser);
        }

        //[Theory]
        //[InlineData(
        //    @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36 OPR/41.0.2353.56",
        //    "Opera 41.0.2353.56")]
        //[InlineData(
        //    @"Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36",
        //    "Chrome 54.0.2840.99")]
        //[InlineData(
        //    @"Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko",
        //    "Internet Explorer 11.0")]
        //public void Build_WhenFileExists_ThenShoulProperlyParseClientInfo(string userAgent, string expectedOutput)
        //{
        //    fileProviderMock.Setup(x => x.GetFileInfo("GovITHub.Auth.Common.Tests.browsers.yml")).Returns(fileInfoMock.Object);

        //    fileInfoMock.SetupGet(x => x.Exists).Returns(false);

        //    loggerMock
        //        .Setup(x => x.Log(It.IsAny<Microsoft.Extensions.Logging.LogLevel>(), It.IsAny<Microsoft.Extensions.Logging.EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
        //        .Callback<Microsoft.Extensions.Logging.LogLevel, Microsoft.Extensions.Logging.EventId, object, Exception, Func<object, Exception, string>>((logLevelCallback, eventIdCallback, objectCallback, exceptionCallback, formatterCallback) =>
        //        {
        //            Assert.Equal(Microsoft.Extensions.Logging.LogLevel.Error, logLevelCallback);
        //            Assert.Equal("Device detector files missing!", formatterCallback(objectCallback, exceptionCallback));
        //        });

        //    var deviceInfo = new DeviceInfo
        //    {
        //        UserAgent = userAgent
        //    };
        //    _builder.Build(deviceInfo, userAgent);
        //    Assert.Equal(expectedOutput, deviceInfo.Browser);
        //}


        public void Dispose()
        {
            fileProviderMock.VerifyAll();
            loggerFactoryMock.VerifyAll();
        }
    }
}