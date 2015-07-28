using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns.Services
{
    public interface IFeaturedCampaignsService : IDependency
    {
        List<CampaignRecord> GetCampaignsFromAdmin();

        List<OrderRecord> GetOrderForOneDay();

        Dictionary<CampaignRecord, int> GetCampaignsFromOrderForOneDay(int[] ids);

        Dictionary<CampaignRecord, int> GetCampaignsFromAdminForOneDay(List<CampaignRecord> camp);

        IQueryable<CampaignRecord> GetAllCampaigns();

        CampaignRecord GetCampaignsById(int id);

        bool UpdateCampaigns(CampaignRecord camp);
    }
}
