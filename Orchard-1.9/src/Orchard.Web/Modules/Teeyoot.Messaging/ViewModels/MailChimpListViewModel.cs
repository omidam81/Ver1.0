using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Messaging.ViewModels
{
    public class MailChimpListViewModel
    {
        public virtual int Id { get; set; } 
        
        public virtual string ApiKey { get; set; }

        public virtual string SellerTemplate { get; set; }

        public virtual string WelcomeTemplate { get; set; }

        public virtual string RelaunchTemplate { get; set; }

        public virtual string LaunchTemplate { get; set; }

        public virtual string CampaignIsPrintingSellerTemplate { get; set; }

        public virtual string OrderIsPrintingBuyerTemplate { get; set; }

        public virtual string CancelledOrderTemplate { get; set; }

        public virtual string DeliveredOrderTemplate { get; set; }

        public virtual string ShippedOrderTemplate { get; set; }

        public virtual string NewOrderTemplate { get; set; }
    
        public virtual string PlaceOrderTemplate { get; set; }

        public virtual string CampaignNotReachGoalSellerTemplate { get; set; }

        public virtual string CampaignNotReachGoalBuyerTemplate { get; set; }

        public virtual string PaidCampaignTemplate { get; set; }

        public virtual string UnpaidCampaignTemplate { get; set; }

        public virtual string PartiallyPaidCampaignTemplate { get; set; }

        public virtual string CampaignPromoTemplate { get; set; }

        public virtual string WithdrawTemplate { get; set; }

        public virtual string Culture { get; set; }

        public virtual IEnumerable<LanguageRecord> AvailableLanguages { get; set; }


    }
}