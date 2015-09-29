using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface ICountryService : IDependency    
    {
        CurrencyRecord GetCurrencyByCulture(string culture);
    }
}
