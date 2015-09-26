using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;

namespace Teeyoot.Localization.GeoLocation
{
    public class WebServiceGeoLocationInfoProvider : IGeoLocationInfoProvider
    {
        private readonly int _userId;
        private readonly string _licenseKey;

        public WebServiceGeoLocationInfoProvider(int userId, string licenseKey)
        {
            _userId = userId;
            _licenseKey = licenseKey;
        }

        private CountryResponse TryGetGeoLocationResponse(string ipAddress)
        {
            using (var client = new WebServiceClient(_userId, _licenseKey))
            {
                var countryResponse = client.Country(ipAddress);
                return countryResponse;
            }
        }

        public GeoLocationInfo GetGeoLocationInfo(string ipAddress)
        {
            try
            {
                var countryResponse = TryGetGeoLocationResponse(ipAddress);
            }
            catch (Exception exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
