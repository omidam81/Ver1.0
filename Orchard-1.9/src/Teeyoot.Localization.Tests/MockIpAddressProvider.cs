using Teeyoot.Localization.IpAddress;

namespace Teeyoot.Localization.Tests
{
    public class MockIpAddressProvider : IIpAddressProvider
    {
        private readonly string _ipAddress;

        public MockIpAddressProvider(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public string GetIpAddress()
        {
            return _ipAddress;
        }
    }
}
