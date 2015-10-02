using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class AdminPromotionViewModel
    {
        public PromotionRecord Promotion { get; set; }

        public string CampaignName { get; set; }

        public int CampaignId { get; set; }

        public string CampaignAlias { get; set; }

        public string CampaignerEmail { get; set; }
    }
}