﻿using System;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;

namespace Teeyoot.Localization.GeoLocation
{
    public class WebServiceGeoLocationInfoProvider : IGeoLocationInfoProvider
    {
        private readonly int _userId;
        private readonly string _licenseKey;
        private const int MaxAttemptsToKnowLocation = 3;

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

        private GeoLocationInfo GetGeoLocationInfo(string ipAddress)
        {
            try
            {
                var countryResponse = GetGeoLocationResponse(ipAddress);
                var geoLocationInfo = new GeoLocationInfo(countryResponse.Country.IsoCode);
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

        private static CountryInfo GetCurrentCountryFrom(GeoLocationInfo geoLocationInfo)
        {
            if (geoLocationInfo.Status != LocationInfoStatus.LocationFound)
            {
                return new CountryInfo {Country = Country.Unknown};
            }

            switch (geoLocationInfo.CountryIsoCode)
            {
                case "MY":
                {
                    return new CountryInfo
                    {
                        Country = Country.Malaysia,
                        CountryIsoCode = geoLocationInfo.CountryIsoCode
                    };
                }
                case "SG":
                {
                    return new CountryInfo
                    {
                        Country = Country.Singapore,
                        CountryIsoCode = geoLocationInfo.CountryIsoCode
                    };
                }
                case "ID":
                {
                    return new CountryInfo
                    {
                        Country = Country.Indonesia,
                        CountryIsoCode = geoLocationInfo.CountryIsoCode
                    };
                }
                default:
                {
                    return new CountryInfo
                    {
                        Country = Country.Other,
                        CountryIsoCode = geoLocationInfo.CountryIsoCode
                    };
                }
            }
        }

        public CountryInfo GetCountry(string ipAddress)
        {
            GeoLocationInfo geoLocationInfo = null;

            for (var i = 0; i < MaxAttemptsToKnowLocation; i++)
            {
                geoLocationInfo = GetGeoLocationInfo(ipAddress);
                switch (geoLocationInfo.Status)
                {
                    case LocationInfoStatus.TransportError:
                    case LocationInfoStatus.InvalidResponse:
                        continue;
                    case LocationInfoStatus.LocationFound:
                    case LocationInfoStatus.UnknownLocation:
                    case LocationInfoStatus.UnknownError:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return GetCurrentCountryFrom(geoLocationInfo);
        }
    }
}
