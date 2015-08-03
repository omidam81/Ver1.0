using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class AdminFeaturedCampaignsViewModel
    {
        public FeaturedCampaignViewModel[] Campaigns { get; set; }

        public dynamic Pager { get; set; }

        public int StartedIndex { get; set; }
    }

    public class FeaturedCampaignViewModel
    {
        public CampaignViewModel Campaign { get; set; }

        public int Last24HoursSold { get; set; }
    }

    public class CampaignViewModel
    {
        public int Id { get; set; }

        public bool IsFeatured { get; set; }

        public int Sold { get; set; }

        public int Goal { get; set; }

        public string Title { get; set; }
    }
}