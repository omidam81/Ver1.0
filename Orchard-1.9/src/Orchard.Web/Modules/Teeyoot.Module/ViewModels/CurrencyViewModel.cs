using Orchard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(50)]
        public string ShortName { get; set; }

        public int? CountryId { get; set; }

        public string FlagFileName { get; set; }

        public HttpPostedFileBase FlagImage { get; set; }

        public IEnumerable<CountryRecord> Countries { get; private set; }

        public string CountryName { get; set; }

        public bool ImageChanged { get; set; }

        public virtual double PriceBuyers { get; set; }

        public virtual double PriceSellers { get; set; }

        public virtual bool IsConvert { get; set; }

        public IEnumerable<CurrencyRecord> CurrenciesNotIsConvert { get; private set; }



        public CurrencyViewModel(IRepository<CountryRecord> countriesRepo)
        {
            Countries = countriesRepo.Table;
        }

        public CurrencyViewModel(IRepository<CurrencyRecord> currenciesRepo)
        {
            CurrenciesNotIsConvert = currenciesRepo.Table.Where( x => x.IsConvert == false);
        }

        public CurrencyViewModel()  
        {
        }
    }
}
