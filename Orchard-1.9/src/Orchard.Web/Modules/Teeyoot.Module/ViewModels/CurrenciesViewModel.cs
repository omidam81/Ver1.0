using System.Collections.Generic;

namespace Teeyoot.Module.ViewModels
{
    public class CurrenciesViewModel
    {
        public IEnumerable<CountryCurrencyItemViewModel> Currencies { get; set; }
        public dynamic Pager { get; set; }
    }
}