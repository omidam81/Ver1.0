﻿using Orchard;
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
        private readonly IRepository<CampaignStatusRecord> _statusRepository;
        private readonly IRepository<CampaignCategoriesRecord> _campaignCategories;
        private readonly IRepository<LinkCampaignAndCategoriesRecord> _linkCampaignAndCategories;


        public CampaignService(IRepository<CampaignRecord> campaignRepository,
                               IRepository<CampaignProductRecord> campProdRepository,
                               IRepository<ProductColorRecord> colorRepository,
                               IRepository<ProductRecord> productRepository,
                               IRepository<CurrencyRecord> currencyRepository,
                               IRepository<CampaignStatusRecord> statusRepository,
                               IRepository<CampaignCategoriesRecord> campaignCategories,
                               IOrchardServices services,
                               IRepository<LinkCampaignAndCategoriesRecord> linkCampaignAndCategories)
        {
            _campaignRepository = campaignRepository;
            _campProdRepository = campProdRepository;
            _colorRepository = colorRepository;
            _productRepository = productRepository;
            _currencyRepository = currencyRepository;
            _statusRepository = statusRepository;
            _campaignCategories = campaignCategories;
            Services = services;
            _linkCampaignAndCategories = linkCampaignAndCategories;
        }

        private IOrchardServices Services { get; set; }


        public IQueryable<CampaignCategoriesRecord> GetAllCategories()
        {
            return _campaignCategories.Table;
        }

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
            return _campaignRepository.Get(id);
        }

        public List<CampaignRecord> GetCampaignsForTheFilter(string filter, int skip = 0, int take = 16, bool tag = false)
        {
            if (tag)
            {
                //var camp = _campaignCategories.Table.Where(c => c.Name.ToLower() == filter).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord)).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).Distinct();
                var categCamp = _campaignCategories.Table.Where(c => c.Name.ToLower() == filter).Select(c => c.Id);
                var campForTags = _linkCampaignAndCategories.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).Distinct();
                return campForTags.Skip(skip).Take(take).ToList();
            }
            else
            {
                var categCamp = _campaignCategories.Table.Where(c => c.Name.ToLower().Contains(filter)).Select(c => c.Id);
                var campForTags = _linkCampaignAndCategories.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord);
                //List<CampaignRecord> campForTags = _campaignCategories.Table.Where(c => c.Name.ToLower().Contains(filter)).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord)).ToList();
                IEnumerable<CampaignRecord> camps = GetAllCampaigns().Where(c => c.Title.Contains(filter) || c.Description.Contains(filter));
                camps = camps.Concat(campForTags).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).Distinct();
                //return camps.Skip(skip).Take(take);
                return camps.Skip(skip).Take(take).ToList();
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
                var newCampaign = new CampaignRecord
                {
                    Alias = data.Alias,
                    BackSideByDefault = data.BackSideByDefault,
                    Description = data.Description,
                    Design = data.Design,
                    EndDate = DateTime.UtcNow.AddDays(data.CampaignLength),
                    StartDate = DateTime.UtcNow,
                    ProductCountGoal = data.ProductCountGoal,
                    ProductCountSold = 0,
                    TeeyootUserId = userId,
                    Title = data.CampaignTitle,
                    CampaignStatusRecord = _statusRepository.Get(1)
                };
                _campaignRepository.Create(newCampaign);

                foreach (var tag in data.Tags)
                {
                    if (_campaignCategories.Table.Where(c => c.Name.ToLower() == tag).FirstOrDefault() != null)
                    {
                        var cat = _campaignCategories.Table.Where(c => c.Name.ToLower() == tag).FirstOrDefault();
                        var link = new LinkCampaignAndCategoriesRecord {
                            CampaignRecord = newCampaign,
                            CampaignCategoriesPartRecord = cat
                        };
                        _linkCampaignAndCategories.Create(link);
                    }
                    else
                    {
                        var cat = new CampaignCategoriesRecord
                        {
                            Name = tag,
                            IsVisible = false
                        };
                        _campaignCategories.Create(cat);
                        var link = new LinkCampaignAndCategoriesRecord
                        {
                            CampaignRecord = newCampaign,
                            CampaignCategoriesPartRecord = cat
                        };
                        _linkCampaignAndCategories.Create(link);
                    }
                }

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

        public void UpdateCampaign(CampaignRecord campiagn) 
        {
            _campaignRepository.Update(campiagn);
        }

        public CampaignProductRecord GetCampaignProductById(int id)
        {
            return _campProdRepository.Get(id);
        }

        public IQueryable<CampaignProductRecord> GetProductsOfCampaign(int campaignId)
        {
            return _campProdRepository.Table.Where(p => p.ProductRecord.Id == campaignId).OrderBy(p => p.Id);
        }

        public IQueryable<CampaignRecord> GetCampaignsOfUser(int userId)
        {
            return GetAllCampaigns(); //TODO: eugene: make for certain user
           //          .Where(c => c.TeeyootUserId == userId);
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

        public bool DeleteCampaignFromCategoryById(int campId, int categId)
        {
            var camp = GetCampaignById(campId);
            try
            {
                foreach (var link in camp.Categories)
                {
                    if (link.CampaignCategoriesPartRecord.Id == categId)
                    {
                        _linkCampaignAndCategories.Delete(link);
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