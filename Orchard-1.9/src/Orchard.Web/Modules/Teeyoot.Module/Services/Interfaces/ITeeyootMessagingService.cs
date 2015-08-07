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

        void SendApproveOrderMessage(string pathToTemplates, string pathToMedia, int orderId);
    }
}
