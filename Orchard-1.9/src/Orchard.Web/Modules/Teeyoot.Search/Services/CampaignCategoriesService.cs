using Orchard.ContentManagement;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.Services
{
    public class CampaignCategoriesService : ICampaignCategoriesService
    {
        private readonly IRepository<CampaignCategoriesPartRecord> _repository;
        private readonly IContentManager _contentManager;

        public CampaignCategoriesService(IRepository<CampaignCategoriesPartRecord> repository, IContentManager contentManager)
        {
            _repository = repository;
            _contentManager = contentManager;
        }

        public IQueryable<CampaignCategoriesPartRecord> GetAllCategories()
        {
            return _repository.Table;
        }

        public IQueryable<CampaignRecord> GetCampaignsByIdCategory(int id)
        {
            return GetAllCategories().Where(c => c.Id == id).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord));
        }

        public CampaignCategoriesPartRecord GetCategoryById(int id)
        {
            return GetAllCategories().Where(c => c.Id == id).FirstOrDefault();
        }

        public IQueryable<CampaignRecord> GetCampaignsByNotThisIdCategory(int id)
        {
            return GetAllCategories().Where(c => c.Id != id).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord));
        }

        public int AddCategory(string name)
        {
            var categ = GetAllCategories().Where(c => c.Name.ToLower() == name.ToLower()).FirstOrDefault();
            if (categ != null && categ.Id > 0)
            {
                return 0;
            }
            else
            {
                var newCateg = _contentManager.Create<CampaignCategoriesPart>(typeof(CampaignCategoriesPart).Name, tb =>
                {
                    tb.Name = name;
                    tb.IsVisible = false;
                });
                return newCateg.Id;
            }
        }
    }
}