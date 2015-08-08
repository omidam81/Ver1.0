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
    }
}
