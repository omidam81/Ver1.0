using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CampaignProductRecord
    {
        public virtual int Id { get; set; }

        public virtual int CampaignRecord_Id { get; set; }

        public virtual double Price { get; set; }

        public virtual double BaseCost { get; set; }

        public virtual ProductRecord ProductRecord { get; set; }

        public virtual ProductColorRecord ProductColorRecord { get; set; }

        public virtual ProductColorRecord SecondProductColorRecord { get; set; }

        public virtual ProductColorRecord ThirdProductColorRecord { get; set; }

        public virtual ProductColorRecord FourthProductColorRecord { get; set; }

        public virtual ProductColorRecord FifthProductColorRecord { get; set; }

        public virtual CurrencyRecord CurrencyRecord { get; set; }
    }
}