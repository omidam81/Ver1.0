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
        private readonly IRepository<LinkCampaignAndCategoriesRecord> _linkCampaignAndCetegory;
        private readonly IContentManager _contentManager;

        public CampaignCategoriesService(IRepository<CampaignCategoriesPartRecord> repository, IContentManager contentManager, IRepository<LinkCampaignAndCategoriesRecord> linkCampaignAndCetegory)
        {
            _repository = repository;
            _contentManager = contentManager;
            _linkCampaignAndCetegory = linkCampaignAndCetegory;
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
                //TODO: don't work
                //var newCateg = _contentManager.Create<CampaignCategoriesPart>("CampaignCategories", tb =>
                //{
                //    tb.Name = name;
                //    tb.IsVisible = false;
                //});
                int newId = GetAllCategories().Select(c => c.Id).Max() + 1;
                var newCateg = new CampaignCategoriesPartRecord
                {
                    Name = name,
                    IsVisible = false
                };
                try
                {
                    _repository.Create(newCateg);
                    return newCateg.Id;
                }catch{
                    return 0;
                }
            }
        }

        public bool CnehgeVisible(int id, bool changes)
        {
            var asd = GetAllCategories().Where(c => c.Id == id).FirstOrDefault();
            asd.IsVisible = changes;
            try
            {
                _repository.Update(asd);
                return true;
            }catch{
                return false;
            }
        }

        public bool DeleteCategory(int id)
        {
            var cat = GetCategoryById(id);
            try
            {
                foreach (var link in cat.Campaigns)
                {
                    _linkCampaignAndCetegory.Delete(link);
                }
                _repository.Delete(cat);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}