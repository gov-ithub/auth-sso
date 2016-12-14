using GovITHub.Auth.Common.Services.DeviceDetection.DataContracts;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders;
using GovITHub.Auth.Common.Services.DeviceDetection.DeviceInfoBuilders.Regexes;
using Moq;
using Xunit;

namespace GovITHub.Auth.Common.Tests.Services.DeviceDetection.DeviceInfoBuilders
{
    public class MobileDeviceInfoBuilderTests : System.IDisposable
    {
        private readonly Mock<Microsoft.Extensions.FileProviders.IFileProvider> fileProviderMock;
        private readonly Mock<Microsoft.Extensions.Logging.ILoggerFactory> loggerFactoryMock;

        private readonly MobileDeviceInfoBuilder _builder;

        public MobileDeviceInfoBuilderTests()
        {
            fileProviderMock = new Mock<Microsoft.Extensions.FileProviders.IFileProvider>(MockBehavior.Strict);
            loggerFactoryMock = new Mock<Microsoft.Extensions.Logging.ILoggerFactory>(MockBehavior.Strict);

            _builder = new MobileDeviceInfoBuilder(new MobileDevicesResourceFileRegexLoader("GovITHub.Auth.Common.Tests.mobiles.yml", fileProviderMock.Object, loggerFactoryMock.Object));
        }

        [Theory]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 4.1.1; HTC One X Build/JRO03C) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19",
            "HTC One X")]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 6.0; HTC One M9 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36",
            "HTC One M9")]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 6.0.1; E6653 Build/32.2.A.0.253) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36",
            "Sony Xperia Z5")]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 6.0.1; Nexus 6P Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.83 Mobile Safari/537.36",
            "Google Nexus 6P")]
        [InlineData(
            @"Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10586",
            "Nokia Lumia 950")]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 6.0.1; SGP771 Build/32.2.A.0.253; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.98 Safari/537.36",
            "Sony SGP771")]
        [InlineData(
            @"Mozilla/5.0 (Linux; Android 5.0.2; SAMSUNG SM-T550 Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/3.3 Chrome/38.0.2125.102 Safari/537.36",
            "Samsung SM-T550")]
        public void ShouldRun(string userAgent, string expectedDeviceType)
        {
            var deviceInfo = new DeviceInfo { UserAgent = userAgent };
            _builder.Build(deviceInfo, userAgent);
            Assert.Equal(expectedDeviceType, deviceInfo.MobileDevice);
        }


        public void Dispose()
        {
            fileProviderMock.VerifyAll();
            loggerFactoryMock.VerifyAll();
        }
    }
}