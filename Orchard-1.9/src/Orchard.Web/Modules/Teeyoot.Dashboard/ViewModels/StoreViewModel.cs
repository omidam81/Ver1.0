using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class StoreViewModel
    {
        public int Id { get; set; }

        public string Img { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public bool HideStore { get; set; }

        public bool CrossSelling { get; set; }

        public IList<CampaignRecord> Campaigns { get; set; }

        public IList<CampaignRecord> SelectedCampaigns { get; set; }

        public StoreViewModel()
        {
            Campaigns = new List<CampaignRecord>();
            SelectedCampaigns = new List<CampaignRecord>();
        }

    }
    
}