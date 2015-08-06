using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }

    public class AdminOrder {
        public int PublicId { get; set; }
        public  IList<LinkOrderCampaignProductRecord> Products { get; set; }
        public string Status { get; set; }
        
        //information buyuer
        public string EmailBuyer { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }

        public string UserNameSeller { get; set; }
        public string Profit { get; set; }
        public bool Payout { get; set; }

    }

}