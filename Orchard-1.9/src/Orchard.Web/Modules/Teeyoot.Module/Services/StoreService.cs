using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<StoreRecord> _storeRepository;
        private readonly IRepository<LinkStoreCampaignRecord> _linkStoreCampaignRecord;


        public StoreService(IRepository<StoreRecord> storeRepository,
                            IRepository<LinkStoreCampaignRecord> linkStoreCampaignRecord)
        {
            _storeRepository = storeRepository;
            _linkStoreCampaignRecord = linkStoreCampaignRecord;
        }


        public IQueryable<StoreRecord> GetAllStoresForUser(int userId)
        {
            return _storeRepository.Table.Where(s => s.TeeyootUserId == userId);
        }

        public StoreRecord GetStoreById(int id)
        {
            return _storeRepository.Table.FirstOrDefault(s => s.Id == id);
        }

        public StoreRecord CreateStore(StoreRecord store)
        {
            try
            {
                //_storeRepository.Create(store);
                //List<LinkStoreCampaignRecord> productsList = new List<LinkStoreCampaignRecord>();

               
                //    var campaignProduct = _campaignService.GetCampaignProductById(product.ProductId);
                //    var orderProduct = new LinkOrderCampaignProductRecord() { Count = product.Count, ProductSizeRecord = _sizeRepository.Get(product.SizeId), CampaignProductRecord = campaignProduct, OrderRecord = order };
                //    _ocpRepository.Create(orderProduct);
                //    productsList.Add(orderProduct);
               

                //order.Products = productsList;
                //return order;
            }
            catch
            {
                throw;
            }

            return new StoreRecord();
           
        }

        public void UpdateStore(StoreRecord store) 
        {
            _storeRepository.Update(store);
        }


        public bool DeleteStore(int id)
        {
            var store = GetStoreById(id);
            try
            {
                foreach (var link in store.Campaigns)
                {
                    if (link.CampaignRecord.Id == id)
                    {
                        _linkStoreCampaignRecord.Delete(link);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}