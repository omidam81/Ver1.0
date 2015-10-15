using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;



namespace Teeyoot.Module.Models
{

    public class CurrencyExchangeRecord
    {
        public virtual int Id { get; protected set; }

        public virtual CurrencyRecord CurrencyFrom { get; set; }
        public virtual CurrencyRecord CurrencyTo { get; set; }

        public virtual double RateForBuyer { get; set; }
        public virtual double RateForSeller { get; set; }
    }
}