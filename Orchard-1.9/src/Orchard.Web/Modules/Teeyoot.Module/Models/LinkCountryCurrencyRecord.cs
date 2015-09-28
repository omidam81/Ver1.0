using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class LinkCountryCurrencyRecord
    {
        public virtual int Id { get; set; }

        public virtual CurrencyRecord CurrencyRecord { get; set; }

        public virtual CountryRecord CountryRecord { get; set; }
    }
}