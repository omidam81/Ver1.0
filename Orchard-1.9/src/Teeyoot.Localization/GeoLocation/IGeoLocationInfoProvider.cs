namespace Teeyoot.Localization.GeoLocation
{
    public interface IGeoLocationInfoProvider
    {
        CountryInfo GetCountry(string ipAddress);
    }
}
