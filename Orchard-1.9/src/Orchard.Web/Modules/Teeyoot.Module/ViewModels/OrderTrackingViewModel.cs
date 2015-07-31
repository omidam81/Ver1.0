using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class OrderTrackingViewModel
    {
        public OrderStatusRecord Status { get; set; }

        public string CampaignName { get; set; }

        public string CampaignAlias { get; set; }

        public string CreateDate { get; set; }

        public LinkOrderCampaignProductRecord[] Products { get; set; }
    }
}