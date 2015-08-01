using Orchard.ContentManagement;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Services
{
    public class CampaignCategoriesService : ICampaignCategoriesService
    {
        private readonly IRepository<CampaignCategoriesRecord> _repository;
        private readonly IRepository<LinkCampaignAndCategoriesRecord> _linkCampaignAndCetegory;
        private readonly IRepository<CampaignRecord> _campaign;

        public CampaignCategoriesService(IRepository<CampaignCategoriesRecord> repository, IRepository<LinkCampaignAndCategoriesRecord> linkCampaignAndCetegory, IRepository<CampaignRecord> campaign)
        {
            _repository = repository;
            _linkCampaignAndCetegory = linkCampaignAndCetegory;
            _campaign = campaign;
        }

        public IQueryable<CampaignCategoriesRecord> GetAllCategories()
        {
            return _repository.Table;
        }


        public IQueryable<CampaignCategoriesRecord> GetCategoryByCampaignId(int id)
        {
            var categoryIds = _linkCampaignAndCetegory.Table.Where( c=> c.CampaignRecord.Id == id).Select(c => c.CampaignCategoriesPartRecord.Id).ToList();

            return GetAllCategories().Where(c => categoryIds.Contains(c.Id));
        }


        public bool CreateCategoriesToCampaign(List<CampaignCategoriesRecord> categories, int campaignId)
        {
            return false;
        }


        public bool UpdateCampaignAndCreateNewCategories(CampaignRecord campaign, List<CampaignCategoriesRecord> categories)
        {
            return false;
        }
    }
}