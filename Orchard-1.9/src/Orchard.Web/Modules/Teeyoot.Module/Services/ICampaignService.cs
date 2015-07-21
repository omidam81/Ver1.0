using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface ICampaignService : IDependency
    {
        IQueryable<CampaignRecord> GetAllCampaigns();

        CampaignRecord GetCampaignByAlias(string alias);

        CampaignRecord GetCampaignById(int id);

        List<CampaignRecord> GetCampaignsForTheFilter(string filter, int skip = 0, int take = 16, bool tag = false);

        CampaignRecord CreateNewCampiagn(LaunchCampaignData data);

        IQueryable<CampaignProductRecord> GetProductsOfCampaign(int campaignId);

        IQueryable<CampaignRecord> GetCampaignsOfUser(int userId);
        
        CampaignProductRecord GetCampaignProductById(int id);

        void UpdateCampaign(CampaignRecord campiagn);
    }
}
