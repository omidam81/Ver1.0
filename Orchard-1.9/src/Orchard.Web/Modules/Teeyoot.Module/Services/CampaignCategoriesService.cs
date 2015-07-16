using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class CampaignCategoriesService : ICampaignCategoriesService
    {
        private readonly IRepository<CampaignCategoriesPartRecord> _repository;

        public CampaignCategoriesService(IRepository<CampaignCategoriesPartRecord> repository)
        {
            _repository = repository;
        }

        public IQueryable<CampaignCategoriesPartRecord> GetAllCategories()
        {
            return _repository.Table;
        }
    }
}