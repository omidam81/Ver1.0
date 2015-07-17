using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public List<CampaignProductRecord> GetCampaignProductsByCampaign(List<CampaignRecord> campList)
        {
            int[] campId = campList.Select(s => s.Id).ToArray();
            return _repository.Table.Where(c => campId.Contains(c.CampaignRecord_Id)).ToList();
        }
    }
}