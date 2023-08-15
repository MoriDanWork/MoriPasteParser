using MoriPattern;
using System.Security.Policy;
using Xunit.Abstractions;

namespace MoriPatternUnit
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void ParseFromClipBoard_ValidInput_ReturnsExpectedProxies()
        {
            // Arrange
            var clipboard = new string[] { "192.168.0.1:8080", "invalid_proxy" };



            await ProxyController.ParseProxyAsync(ParseMode.Clipboard, MoriPattern.Model.ProxyType.HTTPs, _clipboard: clipboard);

            // Act
            var proxies = ProxyController.GetProxies();

            // Assert
            Assert.Single(proxies);
            Assert.Equal("192.168.0.1", proxies[0].Ip);
            Assert.Equal(8080, proxies[0].Port);
        }
        [Fact]
        public async void ParseFromUrl_ValidInput_ReturnsExpectedProxies()
        {
            // Arrange
            string[] url = new string[2] { @"https://githuasdasdab.com/TheSpeedX/PROXY-List/blob/master/socks4.txt", @"https://github.com/TheSpeedX/PROXY-List/blob/master/socks5.txt" };
            await ProxyController.ParseProxyAsync(ParseMode.Url, MoriPattern.Model.ProxyType.HTTPs, _urls: url);

            // Act
            var proxies = ProxyController.GetProxies();

            _output.WriteLine(proxies.Count.ToString());

            Assert.True(proxies.Count > 0);
        }
        [Fact]
        public async void ParseFromFile_ValidInput_ReturnsExpectedProxies()
        {
            // Arrange
            string filePath = "D:\\Desktop\\ProxyCountries_Singapore.txt";
            await ProxyController.ParseProxyAsync(ParseMode.File, MoriPattern.Model.ProxyType.HTTPs, _filePath: filePath);

            // Act
            var proxies = ProxyController.GetProxies();

            _output.WriteLine(proxies.Count.ToString());

            Assert.True(proxies.Count > 0);
        }

    }
}