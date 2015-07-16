using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Search.Models;

namespace Teeyoot.Search.Services
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