﻿using Orchard;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

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
        private readonly IRepository<LinkOrderCampaignProductRecord> _ocpRepository;
        private readonly IRepository<OrderStatusRecord> _orderStatusRepository;
        private readonly IRepository<OrderRecord> _orderRepository;

        public CampaignService(IRepository<CampaignRecord> campaignRepository,
                               IRepository<CampaignProductRecord> campProdRepository,
                               IRepository<ProductColorRecord> colorRepository,
                               IRepository<ProductRecord> productRepository,
                               IRepository<CurrencyRecord> currencyRepository,
                               IRepository<CampaignStatusRecord> statusRepository,
                               IRepository<CampaignCategoriesRecord> campaignCategories,
                               IOrchardServices services,
                               IRepository<LinkCampaignAndCategoriesRecord> linkCampaignAndCategories,
                               IRepository<LinkOrderCampaignProductRecord> ocpRepository,
                               IRepository<OrderStatusRecord> orderStatusRepository,
                               IRepository<OrderRecord> orderRepository)
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
            _ocpRepository = ocpRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderRepository = orderRepository;
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
                    IsForCharity = data.IsForCharity,
                    StartDate = DateTime.UtcNow,
                    ProductCountGoal = data.ProductCountGoal,
                    ProductCountSold = 0,
                    TeeyootUserId = userId,
                    Title = data.CampaignTitle,
                    CampaignStatusRecord = _statusRepository.Table.First(s => s.Name == CampaignStatus.Active.ToString())
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
                    double baseCost = 0;
                    if (!double.TryParse(prod.BaseCost, out baseCost))
                    {
                        double.TryParse(prod.BaseCost.Replace('.', ','), out baseCost);
                    }

                    double price = 0;
                    if (!double.TryParse(prod.Price, out price))
                    {
                        double.TryParse(prod.Price.Replace('.', ','), out price);
                    }

                    var campProduct = new CampaignProductRecord
                    {
                        CampaignRecord_Id = newCampaign.Id,
                        BaseCost = baseCost,
                        CurrencyRecord = prod.CurrencyId != 1 ? _currencyRepository.Get(1) : _currencyRepository.Get(prod.CurrencyId), // TODO: eugene: implement
                        Price = price,
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
            return userId > 0 ? GetAllCampaigns().Where(c => c.TeeyootUserId == userId) : GetAllCampaigns().Where(c => !c.TeeyootUserId.HasValue);
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

        public void CheckExpiredCampaigns()
        {
            var campaigns = _campaignRepository
                                .Table
                                .Where(c => c.EndDate < DateTime.UtcNow && c.CampaignStatusRecord.Name == CampaignStatus.Active.ToString())
                                .ToList();

            foreach (var c in campaigns)
            {
                c.CampaignStatusRecord = _statusRepository
                                            .Table
                                            .First(s => s.Name == CampaignStatus.Ended.ToString());
                _campaignRepository.Update(c);

                var orders = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == c.Id && p.OrderRecord.IsActive).Select(pr => pr.OrderRecord).Distinct().ToList();
                               
                foreach(var o in orders)
                {
                    if (o.OrderStatusRecord.Name == OrderStatus.Reserved.ToString())
                    {
                        o.OrderStatusRecord = c.ProductCountGoal <= c.ProductCountSold ? 
                            _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Printing.ToString()) :
                            _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Cancelled.ToString());
                        o.Paid = DateTime.UtcNow;
                        _orderRepository.Update(o);                           
                    }
                }
                _orderRepository.Flush();
            }
            _campaignRepository.Flush();
        }
    }
}