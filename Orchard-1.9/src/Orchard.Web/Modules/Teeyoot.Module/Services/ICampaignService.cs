using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface ICampaignService
    {
        IQueryable<CampaignRecord> GetAllCampaigns();

        CampaignRecord GetCampaignByAlias(string alias);
    }
}
