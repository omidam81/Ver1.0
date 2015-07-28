using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class AdminFeaturedCampaignsViewModel
    {
        public Dictionary<CampaignRecord, int> AllInFeatured { get; set; }

        public List<int> Color { get; set; }

        //public Dictionary<CampaignRecord, int> CampaignsFromOrderForDay { get; set; }

        //public List<CampaignRecord> OtherCampaigns { get; set; }
    }
}