using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.Services
{
    public interface ICampaignProductService : IDependency
    {
        List<CampaignProductRecord> GetCampaignProductsByCampaign(IEnumerable<int> campaignIds);
    }
}
