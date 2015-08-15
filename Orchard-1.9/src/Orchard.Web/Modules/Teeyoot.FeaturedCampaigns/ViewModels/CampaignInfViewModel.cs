using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class CampaignInfViewModel
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Day { get; set; }
        public int Mounth { get; set; }
        public int Year { get; set; }
        public int Target { get; set; }
        public string Description{ get; set; }
        public IEnumerable<CampaignProductRecord> Products { get; set; }
    }
}