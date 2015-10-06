using System;
using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class AdminOrderViewModel
    {
        public AdminOrderViewModel()
        {
            Orders = new List<AdminOrder>();
        }

        public IList<AdminOrder> Orders { get; set; }
        public dynamic[] DynamicOrders { get; set; }
        public dynamic Pager { get; set; }
        public string SearchString { get; set; }
        public IEnumerable<OrderStatusItemViewModel> OrderStatuses { get; set; }
        public int? SelectedCurrencyFilterId { get; set; }
        public IEnumerable<CurrencyItemViewModel> Currencies { get; set; }
    }

    public class AdminOrder
    {
        public string PublicId { get; set; }
        public IList<LinkOrderCampaignProductRecord> Products { get; set; }
        public string Status { get; set; }



        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public string CampaignAlias { get; set; }

        //information buyuer
        public string EmailBuyer { get; set; }
        public int Id { get; set; }
        public int SellerId { get; set; }
        public DateTime CreateDate { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string StreetAdress { get; set; }
        //public string City { get; set; }
        //public string Country { get; set; }
        //public string PhoneNumber { get; set; }

        public string UserNameSeller { get; set; }
        public double Profit { get; set; }
        public bool Payout { get; set; }
        public string Currency { get; set; }
    }
}