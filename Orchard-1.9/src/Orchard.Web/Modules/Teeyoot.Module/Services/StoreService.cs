using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<StoreRecord> _storeRepository;
        private readonly IRepository<LinkStoreCampaignRecord> _linkStoreCampaignRepository;
        private readonly ICampaignService _campaignService;


        public StoreService(IRepository<StoreRecord> storeRepository,
                            IRepository<LinkStoreCampaignRecord> linkStoreCampaignRepository,
                            ICampaignService campaignService)
        {
            _storeRepository = storeRepository;
            _linkStoreCampaignRepository = linkStoreCampaignRepository;
            _campaignService = campaignService;
        }

        public IQueryable<StoreRecord> GetAllStores()
        {
            return _storeRepository.Table;
        }

        public IQueryable<StoreRecord> GetAllStoresForUser(int userId)
        {
            return _storeRepository.Table.Where(s => s.TeeyootUserId == userId);
        }

        public StoreRecord GetStoreById(int id)
        {
            return _storeRepository.Table.FirstOrDefault(s => s.Id == id);
        }

        public StoreRecord GetStoreByUrl(string url)
        {
            return _storeRepository.Table.FirstOrDefault(s => s.Url == url);
        }

        public StoreRecord CreateStore(int? teeyootUserId, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns)
        {
            try
            {
                var store = new StoreRecord()
                {
                    Title = title,
                    Description = description,
                    CrossSelling = crossSelling,
                    HideStore = hideStore,
                    TeeyootUserId = teeyootUserId,
                    Url = url

                };
                
                _storeRepository.Create(store);
                List<LinkStoreCampaignRecord> campaigns = new List<LinkStoreCampaignRecord>();

                foreach (var campaignId in selectedCampaigns)
                {
                    var campaign = _campaignService.GetCampaignById(int.Parse(campaignId));
                    var storeCampaign = new LinkStoreCampaignRecord() { CampaignRecord = campaign, StoreRecord = store };
                    _linkStoreCampaignRepository.Create(storeCampaign);
                    campaigns.Add(storeCampaign);
                }

                store.Campaigns = campaigns;
                return store;
            }
            catch
            {
                throw;
            }         
        }

        public void UpdateStore(int id, int? teeyootUserId, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns)
        {             
            try
            {
                var store = GetStoreById(id);
                store.Title = title;
                store.Description = description;
                store.Url = url;
                store.HideStore = hideStore;
                store.CrossSelling = crossSelling;

                foreach (var link in store.Campaigns)
                {                  
                        _linkStoreCampaignRepository.Delete(link);                       
                }

                List<LinkStoreCampaignRecord> campaigns = new List<LinkStoreCampaignRecord>();

                foreach (var campaignId in selectedCampaigns)
                {
                    var campaign = _campaignService.GetCampaignById(int.Parse(campaignId));
                    var storeCampaign = new LinkStoreCampaignRecord() { CampaignRecord = campaign, StoreRecord = store };
                    _linkStoreCampaignRepository.Create(storeCampaign);
                    campaigns.Add(storeCampaign);
                }

                store.Campaigns = campaigns;

                _storeRepository.Update(store);
            }
            catch
            {
                throw;
            }      
        }

        public bool DeleteStore(int id)
        {
            var store = GetStoreById(id);
            try
            {
                foreach (var link in store.Campaigns)
                {                  
                        _linkStoreCampaignRepository.Delete(link);                       
                }
                _storeRepository.Delete(store);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}