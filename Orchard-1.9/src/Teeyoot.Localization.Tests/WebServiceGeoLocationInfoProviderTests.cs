using NUnit.Framework;
using Teeyoot.Localization.GeoLocation;

namespace Teeyoot.Localization.Tests
{
    [TestFixture]
    public class WebServiceGeoLocationInfoProviderTests
    {
        private IGeoLocationInfoProvider _geoLocationInfoProvider;
        private readonly string[] _unitedStatesIpAddresses = {"72.14.192.0", "209.85.255.255", "216.239.32.0", "66.102.15.255"};
        private readonly string[] _malaysianIpAddresses = {"58.71.128.0", "113.210.0.0", "120.139.255.255", "14.1.255.255"};
        private readonly string[] _indonesianIpAddresses = {"61.247.0.0", "110.232.64.0", "114.59.255.255", "124.81.255.255"};

        [TestFixtureSetUp]
        public void SetUp()
        {
            _geoLocationInfoProvider = new WebServiceGeoLocationInfoProvider(105065, "QhPUJhXH8sh8");
        }

        [Test]
        public void Malaysia()
        {
            foreach (var ipAddress in _malaysianIpAddresses)
            {
                Assert.AreEqual(Country.Malaysia, _geoLocationInfoProvider.GetCountry(ipAddress).Country);
            }
        }

        [Test]
        public void Indonesia()
        {
            foreach (var ipAddress in _indonesianIpAddresses)
            {
                Assert.AreEqual(Country.Indonesia, _geoLocationInfoProvider.GetCountry(ipAddress).Country);
            }
        }

        [Test]
        public void OtherCountries()
        {
            foreach (var ipAddress in _unitedStatesIpAddresses)
            {
                Assert.AreEqual(Country.Other, _geoLocationInfoProvider.GetCountry(ipAddress).Country);
            }
        }

        [Test]
        public void LocalHost()
        {
            Assert.AreEqual(Country.Unknown, _geoLocationInfoProvider.GetCountry("::1").Country);
        }
    }
}
