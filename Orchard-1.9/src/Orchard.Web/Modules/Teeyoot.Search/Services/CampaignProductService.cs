using System.Collections.Generic;
using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.Services
{
    public class CampaignProductService : ICampaignProductService
    {
        private readonly IRepository<CampaignProductRecord> _repository;

        public CampaignProductService(IRepository<CampaignProductRecord> repository)
        {
            _repository = repository;
        }

        public List<CampaignProductRecord> GetCampaignProductsByCampaign(IEnumerable<int> campaignIds)
        {
            return _repository.Table
                .Where(c => campaignIds.Contains(c.CampaignRecord_Id))
                .ToList();
        }
    }
}