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

        IQueryable<CampaignRecord> GetCampaignsForTheFilter(string filter, int skip, int take);
    }
}
