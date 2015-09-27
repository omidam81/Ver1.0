using Teeyoot.Localization.GeoLocation;

namespace Teeyoot.Localization
{
    public interface ILocalizationInfo
    {
        GeoLocationInfo GeoLocationInfo { get; }
        Country Country { get; }
    }
}
