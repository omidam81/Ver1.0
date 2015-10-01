using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public int? CountryId { get; set; }

        public string FlagFileName { get; set; }

        public HttpPostedFileBase FlagImage { get; set; }

        public IEnumerable<CountryRecord> Countries { get; private set; }

        public string CountryName { get; set; }

        public bool ImageChanged { get; set; }

        public CurrencyViewModel(IRepository<CountryRecord> countriesRepo)
        {
            Countries = countriesRepo.Table;
        }

        public CurrencyViewModel()  
        {
        }
    }
}
