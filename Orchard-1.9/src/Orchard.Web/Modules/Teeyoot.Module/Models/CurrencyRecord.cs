﻿using System.Collections.Generic;
using Orchard.Data.Conventions;

namespace Teeyoot.Module.Models
{
    public class CurrencyRecord
    {
        public virtual int Id { get; protected set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string ShortName { get; set; }
        //todo: drop this field 
        public virtual string CurrencyCulture { get; set; }
        public virtual string FlagFileName { get; set; }
        public virtual double PriceBuyers { get; set; }
        public virtual double PriceSellers { get; set; }
        public virtual bool IsConvert { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual IList<LinkCountryCurrencyRecord> CountryCurrencies { get; set; }

        public CurrencyRecord()
        {
            CountryCurrencies = new List<LinkCountryCurrencyRecord>();
        }
    }
}