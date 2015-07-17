using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<CampaignRecord> _repository;

        public CampaignService(IRepository<CampaignRecord> repository)
	    {
            _repository = repository;
	    }

        public IQueryable<CampaignRecord> GetAllCampaigns()
        {
            return _repository.Table;
        }

        public CampaignRecord GetCampaignByAlias(string alias)
        {
            return GetAllCampaigns().FirstOrDefault(c => c.Alias == alias);
        }

        public IQueryable<CampaignRecord> GetCampaignsForTheFilter(string filter, int skip = 0, int take = 16, bool tag = false)
        {
            if (tag)
            {
                return GetAllCampaigns().Where(c => c.Tags.Contains(filter)).OrderByDescending(c => c.ProductCountSold).Skip(0).Take(16);
            }
            else
            {
                return GetAllCampaigns().Where(c => c.Title.Contains(filter) || c.Description.Contains(filter) || c.Tags.Contains(filter)).OrderByDescending(c => c.ProductCountSold).Skip(skip).Take(take);
            }
        }
    }
}