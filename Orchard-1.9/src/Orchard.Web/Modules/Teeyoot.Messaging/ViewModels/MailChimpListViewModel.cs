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

        public virtual string RelaunchApprovedSellerTemplate { get; set; }

        public virtual string RelaunchApprovedBuyerTemplate { get; set; }

        public virtual string RelaunchAdminSellerTemplate { get; set; }

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

        public virtual string AllOrderDeliveredTemplate { get; set; }

        public virtual string CampaignIsFinishedTemplate { get; set; }

        public virtual string DefinitelyGoSellerTemplate { get; set; }

        public virtual string DefinitelyGoBuyerTemplate { get; set; }

        public virtual string EditedCampaignTemplate { get; set; }

        public virtual string ExpiredMetMinimumTemplate { get; set; }

        public virtual string ExpiredNotSuccessfullTemplate { get; set; }

        public virtual string ExpiredSuccessfullTemplate { get; set; }

        public virtual string MakeTheCampaignTemplate { get; set; }

        public virtual string NewCampaignAdminTemplate { get; set; }

        public virtual string NewOrderBuyerTemplate { get; set; }

        public virtual string NotReachGoalMetMinimumTemplate { get; set; }

        public virtual string RecoverOrdersTemplate { get; set; }

        public virtual string RejectTemplate { get; set; }

        public virtual string Shipped3DayAfterTemplate { get; set; }

        public virtual string TermsConditionsTemplate { get; set; }

        public virtual string WithdrawCompletedTemplate { get; set; }

        public virtual string WithdrawSellerTemplate { get; set; }
    }
}