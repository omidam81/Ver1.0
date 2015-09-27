using Teeyoot.Localization.GeoLocation;
using Teeyoot.Localization.IpAddress;

namespace Teeyoot.Localization.LocalizationStorage
{
    public static class LocalizationInfoFactory
    {
        private static IIpAddressProvider _ipAddressProvider;
        private static IGeoLocationInfoProvider _geoLocationInfoProvider;

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
            var country = _geoLocationInfoProvider.GetCountry(ipAddress);

            return new TeeyootLocalizationInfo {Country = country};
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
