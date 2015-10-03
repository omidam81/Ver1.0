using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class CurrenciesViewModel
    {
        public IEnumerable<CurrencyViewModel> Currencies { get; set; }

        public dynamic Pager { get; set; }
    }
}


