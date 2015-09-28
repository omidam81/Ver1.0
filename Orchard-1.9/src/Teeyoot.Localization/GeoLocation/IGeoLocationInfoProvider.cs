namespace Teeyoot.Localization.GeoLocation
{
    public interface IGeoLocationInfoProvider
    {
        Country GetCountry(string ipAddress);
    }
}
