using Orchard.Data.Conventions;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class CampaignRelRequestViewModel
    {
        public string CampaignTitle { get; set; }

        public int CampaignId { get; set; }

        public string CampaignAlias { get; set; }

        public UserPartRecord Seller { get; set; }

        public int CntRequests { get; set; }

        public List<BringBackCampaignRecord> Requests { get; set; }


    }
}