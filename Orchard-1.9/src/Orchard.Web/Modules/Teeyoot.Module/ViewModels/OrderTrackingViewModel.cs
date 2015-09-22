using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class OrderTrackingViewModel
    {
        public int OrderId { get; set; }

        public string OrderPublicId { get; set; }

        public OrderStatusRecord Status { get; set; }

        public string CampaignName { get; set; }

        public string CampaignAlias { get; set; }

        public string CreateDate { get; set; }

        public LinkOrderCampaignProductRecord[] Products { get; set; }

        public OrderHistoryRecord[] Events { get; set; }

        public string[] ShippingTo { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public string TotalPrice { get; set; }

        public string Delivery { get; set; }

        public string Promotion { get; set; }
    }
}