using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Models
{
    public class LaunchCampaignData
    {
        // come from front-end now
        public int ProductCountGoal { get; set; }
        public string CampaignTitle { get; set; }
        public string Description { get; set; }
        public int CampaignLength { get; set; }
        public string Alias { get; set; }
        public bool BackSideByDefault { get; set; }

        // needed as well
        [AllowHtml]
        public string Design { get; set; }
        public string[] Tags { get; set; }
        public CampaignProductData[] Products { get; set; }
    }

    public class CampaignProductData
    {
        public int CurrencyId { get; set; }

        public int ProductId { get; set; }

        public int ColorId { get; set; }

        public double BaseCost { get; set; }

        public double Price { get; set; }
    }
}