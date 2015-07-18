using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<CampaignRecord> _campaignRepository;
        private readonly IRepository<CampaignProductRecord> _campProdRepository;
        private readonly IRepository<ProductColorRecord> _colorRepository;
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IRepository<CurrencyRecord> _currencyRepository;

        public CampaignService(IRepository<CampaignRecord> campaignRepository, 
                               IRepository<CampaignProductRecord> campProdRepository, 
                               IRepository<ProductColorRecord> colorRepository,
                               IRepository<ProductRecord> productRepository,
                               IRepository<CurrencyRecord> currencyRepository,
                               IOrchardServices services)
	    {
            _campaignRepository = campaignRepository;
            _campProdRepository = campProdRepository;
            _colorRepository = colorRepository;
            _productRepository = productRepository;
            _currencyRepository = currencyRepository;
            Services = services;
            
	    }

        private IOrchardServices Services { get; set; }


        public IQueryable<CampaignRecord> GetAllCampaigns()
        {
            return _campaignRepository.Table;
        }

        public CampaignRecord GetCampaignByAlias(string alias)
        {
            return GetAllCampaigns().FirstOrDefault(c => c.Alias == alias);
        }

        public CampaignRecord GetCampaignById(int id)
        {
            return GetAllCampaigns().FirstOrDefault(c => c.Id == id);
        }

        public IQueryable<CampaignRecord> GetCampaignsForTheFilter(string filter, int skip = 0, int take = 16, bool tag = false)
        {
            if (tag)
            {
                return GetAllCampaigns().Where(c => c.Tags.Contains(filter)).OrderByDescending(c => c.ProductCountSold).Skip(0).Take(16);
            }
            else
            {
                return GetAllCampaigns().Where(c => c.Title.Contains(filter) || c.Description.Contains(filter) || c.Tags.Contains(filter)).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).Skip(skip).Take(take);
            }
        }

        public CampaignRecord CreateNewCampiagn(LaunchCampaignData data)
        {
            // TODO: eugene: implement with real data, real user

            FillWithFakeData(data); // TODO: eugene: get rid of this

            var user = Services.WorkContext.CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            int? userId = null;

            if (teeyootUser != null)
            {
                userId = teeyootUser.ContentItem.Record.Id;
            }
           
            try 
            {

                //TODO: Viktor: implement Tags

                var newCampaign = new CampaignRecord
                {
                    Alias = data.Alias,
                    BackSideByDefault = data.BackSideByDefault,
                    Description = data.Description,
                    Design = data.Design,
                    EndDate = DateTime.Now.AddDays(data.CampaignLength),
                    ProductCountGoal = data.ProductCountGoal,
                    ProductCountSold = 0,
                    TeeyootUserId = userId,
                    Title = data.CampaignTitle,
                };
                _campaignRepository.Create(newCampaign);

                foreach (var prod in data.Products)
                {
                    var campProduct = new CampaignProductRecord
                    {
                        CampaignRecord_Id = newCampaign.Id,
                        BaseCost = prod.BaseCost,
                        CurrencyRecord = _currencyRepository.Get(prod.CurrencyId),
                        Price = prod.Price,
                        ProductColorRecord = _colorRepository.Get(prod.ColorId),
                        ProductRecord = _productRepository.Get(prod.ProductId)
                    };

                    _campProdRepository.Create(campProduct);

                    newCampaign.Products.Add(campProduct);
                }

                return newCampaign;
            }
            catch
            {
                throw;
            }
        }

        public IQueryable<CampaignProductRecord> GetProductsOfCampaign(int campaignId)
        {
            return _campProdRepository.Table.Where(p => p.ProductRecord.Id == campaignId).OrderBy(p => p.Id);
        }

        private void FillWithFakeData(LaunchCampaignData data)
        {
            data.Design = "{ some data in Json }";

            var prodCount = new Random().Next(1, 3);

            if (prodCount == 2)
            {
                data.Products = new CampaignProductData[] {

                    new CampaignProductData {
                        CurrencyId = 1,
                        ProductId = 2,
                        ColorId = 3,
                        BaseCost = 10,
                        Price = 15
                    },

                    new CampaignProductData {
                        CurrencyId = 1,
                        ProductId = 3,
                        ColorId = 9,
                        BaseCost = 15,
                        Price = 20
                    }
                };
            }
            else
            {
                data.Products = new CampaignProductData[] {

                    new CampaignProductData {
                        CurrencyId = 1,
                        ProductId = 2,
                        ColorId = 3,
                        BaseCost = 10,
                        Price = 15
                    }
                };
            }
        }
    }
}