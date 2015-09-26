namespace Teeyoot.Localization.GeoLocation
{
    public interface IGeoLocationInfoProvider
    {
        GeoLocationInfo GetGeoLocationInfo(string ipAddress);
    }
}
