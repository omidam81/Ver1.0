namespace Teeyoot.Localization
{
    public class TeeyootLocalizationInfo : ILocalizationInfo
    {
        public Country Country { get; private set; }
        public string CountryIsoCode { get; private set; }

        internal TeeyootLocalizationInfo(CountryInfo countryInfo)
        {
            Country = countryInfo.Country;
            CountryIsoCode = countryInfo.CountryIsoCode;
        }
    }
}
