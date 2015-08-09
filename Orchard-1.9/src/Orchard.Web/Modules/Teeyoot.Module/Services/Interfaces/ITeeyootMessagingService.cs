using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using Mandrill.Model;
using Mandrill;

namespace Teeyoot.Module.Services
{
    public interface ITeeyootMessagingService : IDependency
    {
        void SendLaunchCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId);

        void SendOrderStatusMessage(string pathToTemplates, string pathToMedia, int orderId, string orderStatus);

        void SendExpiredCampaignMessageToSeller(int campaignId, bool isSuccesfull);

        void SendExpiredCampaignMessageToBuyers(int campaignId, bool isSuccesfull);

        void SendSellerMessage(int messageId, string pathToMedia, string pathToTemplates);

        void SendNewOrderMessageToAdmin(int orderId);

        void SendChangedCampaignStatusMessage(int campaignId, string campaignStatus);

        void SendPayoutRequestMessageToAdmin(int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin);
    }
}
