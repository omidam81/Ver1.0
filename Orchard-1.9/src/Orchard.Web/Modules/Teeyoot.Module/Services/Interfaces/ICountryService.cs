using System.Linq;
using Orchard;
using Orchard.Localization.Records;
using Teeyoot.Localization;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface ICountryService : IDependency    
    {
        IQueryable<CountryRecord> GetAllCountry();
        CurrencyRecord GetCurrencyByCulture(string culture);
        CountryRecord GetCountryByCulture(string culture);
        IQueryable<CultureRecord> GetCultureByCountry(int countryId);
        IQueryable<LinkCountryCultureRecord> GetAllCultureWithAllCountry();
        CountryRecord GetCountry(ILocalizationInfo localizationInfo);
        CurrencyRecord GetCurrency(ILocalizationInfo localizationInfo);
    }
}
