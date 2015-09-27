using System;
using Teeyoot.Localization.GeoLocation;
using Teeyoot.Localization.IpAddress;

namespace Teeyoot.Localization.LocalizationStorage
{
    public static class LocalizationInfoFactory
    {
        private static IIpAddressProvider _ipAddressProvider;
        private static IGeoLocationInfoProvider _geoLocationInfoProvider;
        private const int MaxAttemptsToKnowLocation = 3;

        public static void Init(
            IIpAddressProvider ipAddressProvider,
            IGeoLocationInfoProvider geoLocationInfoProvider)
        {
            _ipAddressProvider = ipAddressProvider;
            _geoLocationInfoProvider = geoLocationInfoProvider;
        }

        private static ILocalizationInfo GetNewLocalizationInfo()
        {
            var ipAddress = _ipAddressProvider.GetIpAddress();
            var geoLocationInfo = GetGeoLocationInfo(ipAddress);
            var country = GetCurrentCountryFrom(geoLocationInfo);

            var localizationInfo = new TeeyootLocalizationInfo
            {
                GeoLocationInfo = geoLocationInfo,
                Country = country
            };

            return localizationInfo;
        }

        private static GeoLocationInfo GetGeoLocationInfo(string ipAddress)
        {
            GeoLocationInfo geoLocationInfo = null;

            for (var i = 0; i < MaxAttemptsToKnowLocation; i++)
            {
                geoLocationInfo = _geoLocationInfoProvider.GetGeoLocationInfo(ipAddress);
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

            return geoLocationInfo;
        }

        private static Country GetCurrentCountryFrom(GeoLocationInfo geoLocationInfo)
        {
            if (geoLocationInfo.Status != LocationInfoStatus.LocationFound) 
                return Country.Unknown;

            switch (geoLocationInfo.CountryIsoCode)
            {
                case "MY":
                    return Country.Malaysia;
                case "SG":
                    return Country.Singapore;
                case "ID":
                    return Country.Indonesia;
                default:
                    return Country.Other;
            }
        }

        public static ILocalizationInfo GetCurrentLocalizationInfo()
        {
            var localizationStorageContainer = LocalizationInfoStorageContainerFactory.GetStorageContainer();

            var localizationInfo = localizationStorageContainer.GetCurrentLocalizationInfo();

            if (localizationInfo != null)
                return localizationInfo;

            localizationInfo = GetNewLocalizationInfo();
            localizationStorageContainer.Store(localizationInfo);

            return localizationInfo;
        }
    }
}
