using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using Mandrill.Model;
using Mandrill;
using System.Collections.Generic;

namespace Teeyoot.Module.Services
{
    public interface ITeeyootMessagingService : IDependency
    {
        void SendCheckoutRequestEmails(IEnumerable<CheckoutCampaignRequest> checkoutCampaignRequests);

        void SendLaunchCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId);

        void SendReLaunchCampaignMessageToAdmin(int campaignId);

        void SendReLaunchCampaignMessageToSeller(int campaignId);

        void SendOrderStatusMessage(string pathToTemplates, string pathToMedia, int orderId, string orderStatus);

        void SendExpiredCampaignMessageToSeller(int campaignId, bool isSuccesfull);
       
        void SendExpiredCampaignMessageToAdmin(int campaignId, bool isSuccesfull);

        void SendExpiredCampaignMessageToBuyers(int campaignId, bool isSuccesfull);

        void SendSellerMessage(int messageId, string pathToMedia, string pathToTemplates);

        void SendNewOrderMessageToAdmin(int orderId, string pathToMedia, string pathToTemplates);

        void SendChangedCampaignStatusMessage(int campaignId, string campaignStatus);

        void SendPayoutRequestMessageToAdmin(int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin);

        void SendRecoverOrderMessage(string pathToTemplates, IList<OrderRecord> orders, string email);

        void SendCompletedPayoutMessage(string pathToTemplates, string pathToMedia, PayoutRecord payout);

        void SendNewOrderMessageToBuyer(int orderId, string pathToMedia, string pathToTemplates);

        void SendNewCampaignAdminMessage(string pathToTemplates, string pathToMedia, int campaignId);

        void SendRejectedCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId);

        void SendEditedCampaignMessageToSeller(int campaignId, string pathToMedia, string pathToTemplates);

        void SendCampaignMetMinimumMessageToBuyers(int campaignId);

        void SendCampaignMetMinimumMessageToSeller(int campaignId);

        void SendAllOrderDeliveredMessageToSeller(int campaignId);

        void SendTermsAndConditionsMessageToSeller();

        void SendCampaignFinished1DayMessageToSeller();

        void SendOrderShipped3DaysToBuyer();

        void SendReLaunchApprovedCampaignMessageToSeller(string pathToTemplates, string pathToMedia, int campaignId);

        void SendReLaunchApprovedCampaignMessageToBuyers(string pathToTemplates, string pathToMedia, int campaignId);
    }
}
