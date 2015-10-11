using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Orchard.Data;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class CampaignInfViewModel
    {
        public int CampaignId { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Day { get; set; }
        public int Mounth { get; set; }
        public int Year { get; set; }
        public int Target { get; set; }
        public string Description{ get; set; }
        public CurrencyRecord Currency { get; set; }
        public IEnumerable<CampaignProductRecord> Products { get; set; }
        public IRepository<CurrencyRecord> Currencies { get; set; }
    }
}