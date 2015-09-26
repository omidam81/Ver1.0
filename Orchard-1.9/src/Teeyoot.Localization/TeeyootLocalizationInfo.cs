using Teeyoot.Localization.GeoLocation;

namespace Teeyoot.Localization
{
    public class TeeyootLocalizationInfo : ILocalizationInfo
    {
        public GeoLocationInfo GeoLocationInfo { get; internal set; }
    }
}
