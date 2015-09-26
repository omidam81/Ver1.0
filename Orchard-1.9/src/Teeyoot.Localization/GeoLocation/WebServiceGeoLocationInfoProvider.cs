using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
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

        private CountryResponse GetGeoLocationResponse(string ipAddress)
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
                var countryResponse = GetGeoLocationResponse(ipAddress);
                var geoLocationInfo = new GeoLocationInfo {CountryIsoCode = countryResponse.Country.IsoCode};
                return geoLocationInfo;
            }
            catch (AddressNotFoundException exception)
            {
                return new GeoLocationInfo(LocationInfoStatus.UnknownLocation, exception.Message);
            }
            catch (HttpException exception)
            {
                return new GeoLocationInfo(LocationInfoStatus.TransportError, exception.Message);
            }
            catch (GeoIP2Exception exception)
            {
                return new GeoLocationInfo(LocationInfoStatus.InvalidResponse, exception.Message);
            }
            catch (Exception exception)
            {
                return new GeoLocationInfo(LocationInfoStatus.UnknownError, exception.Message);
            }
        }
    }
}
