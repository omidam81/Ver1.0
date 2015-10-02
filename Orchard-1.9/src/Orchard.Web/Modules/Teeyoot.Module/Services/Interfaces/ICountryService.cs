using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Teeyoot.Module.Models;
using Orchard.Localization.Records;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface ICountryService : IDependency    
    {
        IQueryable<CountryRecord> GetAllCountry();

        CurrencyRecord GetCurrencyByCulture(string culture);

        CountryRecord GetCountryByCulture(string culture);

        IQueryable<CultureRecord> GetCultureByCountry(int countryId);

        IQueryable<LinkCountryCultureRecord> GetAllCultureWithAllCountry();
    }
}
