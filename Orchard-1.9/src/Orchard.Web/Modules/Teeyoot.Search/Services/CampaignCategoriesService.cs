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
        private readonly IRepository<CampaignCategoriesRecord> _repository;
        private readonly IRepository<LinkCampaignAndCategoriesRecord> _linkCampaignAndCetegory;
        private readonly IContentManager _contentManager;
        private readonly IRepository<CampaignRecord> _campaign;

        public CampaignCategoriesService(IRepository<CampaignCategoriesRecord> repository, IContentManager contentManager, IRepository<LinkCampaignAndCategoriesRecord> linkCampaignAndCetegory, IRepository<CampaignRecord> campaign)
        {
            _repository = repository;
            _contentManager = contentManager;
            _linkCampaignAndCetegory = linkCampaignAndCetegory;
            _campaign = campaign;
        }

        public IQueryable<CampaignCategoriesRecord> GetAllCategories()
        {
            return _repository.Table;
        }

        public IQueryable<CampaignRecord> GetCampaignsByIdCategory(int id)
        {
            var categCamp = GetAllCategories().Where(c => c.Id == id).Select(c => c.Id);
            var campForTags = _linkCampaignAndCetegory.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord);
            //return GetAllCategories().Where(c => c.Id == id).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord));
            return campForTags;
        }

        public CampaignCategoriesRecord GetCategoryById(int id)
        {
            return GetAllCategories().Where(c => c.Id == id).FirstOrDefault();
        }

        public List<CampaignRecord> GetCampaignsByNotThisIdCategory(int id)
        {
            //var categCamp = _campaignCategories.Table.Where(c => c.Name.ToLower().Contains(filter)).Select(c => c.Id);
            //var campForTags = _linkCampaignAndCategories.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord);

            var allCampaigns = _campaign.Table.ToList();
            //var campInCateg = _repository.Table.Where(c => c.Id == id).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord)).Distinct().ToList();
            var categCamp = _repository.Table.Where(c => c.Id == id).Select(c => c.Id);
            var campForTags = _linkCampaignAndCetegory.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord);
            foreach (var s in campForTags)
            {
                allCampaigns.Remove(s);
            }

            return allCampaigns;
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
                var newCateg = new CampaignCategoriesRecord
                {
                    Name = name,
                    IsVisible = false
                };
                try
                {
                    _repository.Create(newCateg);
                    return newCateg.Id;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public bool CnehgeVisible(int id, bool changes)
        {
            var cat = GetAllCategories().Where(c => c.Id == id).FirstOrDefault();
            cat.IsVisible = changes;
            try
            {
                _repository.Update(cat);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCategory(int id)
        {
            var cat = GetCategoryById(id);
            try
            {
                var links = _linkCampaignAndCetegory.Table.Where(c => c.CampaignCategoriesPartRecord.Id == id).ToList();
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        _linkCampaignAndCetegory.Delete(link);
                    }
                }
                _repository.Delete(cat);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChnageNameCategory(int id, string newName)
        {
            var cat = GetCategoryById(id);
            cat.Name = newName;
            try
            {
                _repository.Update(cat);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddCampaignToCategory(int idCamp, int idCateg)
        {
            var camp = _campaign.Table.Where(c => c.Id == idCamp).FirstOrDefault();
            var categ = GetCategoryById(idCateg);

            var link = new LinkCampaignAndCategoriesRecord
            {
                CampaignRecord = camp,
                CampaignCategoriesPartRecord = categ
            };
            try
            {
                _linkCampaignAndCetegory.Create(link);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}