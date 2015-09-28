namespace Teeyoot.Localization
{
    public class TeeyootLocalizationInfo : ILocalizationInfo
    {
        public Country Country { get; private set; }

        internal TeeyootLocalizationInfo(Country country)
        {
            Country = country;
        }
    }
}
