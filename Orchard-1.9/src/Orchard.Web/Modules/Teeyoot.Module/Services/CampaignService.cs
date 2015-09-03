using Braintree;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
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
        private readonly IRepository<OrderHistoryRecord> _orderHistoryRepository;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IRepository<BringBackCampaignRecord> _backCampaignRepository;

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
                               IRepository<OrderRecord> orderRepository,
                               IRepository<OrderHistoryRecord> orderHistoryRepository,
                               ITeeyootMessagingService teeyootMessagingService,
                               IRepository<BringBackCampaignRecord> backCampaignRepository)
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
            _orderHistoryRepository = orderHistoryRepository;
            _teeyootMessagingService = teeyootMessagingService;
            _backCampaignRepository = backCampaignRepository;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }


        public BraintreeGateway Gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            PublicKey = "ny4y8s7fkcvnfw9t",
            PrivateKey = "1532863effa7197329266f7de4837bae",
            MerchantId = "7qw5pmrj3hqd2hr4"
        };

        public IQueryable<CampaignCategoriesRecord> GetAllCategories()
        {
            return _campaignCategories.Table;
        }

        public IQueryable<CampaignRecord> GetAllCampaigns()
        {
            return _campaignRepository.Table.Where(c => c.WhenDeleted == null);
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
                var campForTags = _linkCampaignAndCategories.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord).Where(c => c.WhenDeleted == null && !c.IsPrivate && c.IsActive && c.IsApproved).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).Distinct();
                return campForTags.Skip(skip).Take(take).ToList();
            }
            else
            {
                var categCamp = _campaignCategories.Table.Where(c => c.Name.ToLower().Contains(filter)).Select(c => c.Id);
                var campForTags = _linkCampaignAndCategories.Table.Where(c => categCamp.Contains(c.CampaignCategoriesPartRecord.Id)).Select(c => c.CampaignRecord).Where(c => c.WhenDeleted == null && !c.IsPrivate && c.IsActive && c.IsApproved);
                //List<CampaignRecord> campForTags = _campaignCategories.Table.Where(c => c.Name.ToLower().Contains(filter)).SelectMany(c => c.Campaigns.Select(x => x.CampaignRecord)).ToList();
                IEnumerable<CampaignRecord> camps = GetAllCampaigns().Where(c => !c.IsPrivate && c.IsActive && c.IsApproved).Where(c => c.Title.Contains(filter) || c.Description.Contains(filter));
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
                    IsActive = true,
                    IsApproved = false,
                    CampaignStatusRecord = _statusRepository.Table.First(s => s.Name == CampaignStatus.Unpaid.ToString()),
                    CampaignProfit = data.CampaignProfit != null ? data.CampaignProfit : string.Empty,
                    ProductMinimumGoal = data.ProductMinimumGoal == 0 ? 1 : data.ProductMinimumGoal,
                    CampaignCulture = (data.CampaignCulture == null || string.IsNullOrEmpty(data.CampaignCulture)) ? "en-MY" : data.CampaignCulture.Trim()
                };
                _campaignRepository.Create(newCampaign);

                if (data.Tags != null)
                {
                    foreach (var tag in data.Tags)
                    {
                        if (_campaignCategories.Table.Where(c => c.Name.ToLower() == tag).FirstOrDefault() != null)
                        {
                            var cat = _campaignCategories.Table.Where(c => c.Name.ToLower() == tag).FirstOrDefault();
                            var link = new LinkCampaignAndCategoriesRecord
                            {
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
                        ProductRecord = _productRepository.Get(prod.ProductId),
                        SecondProductColorRecord = prod.SecondColorId == 0 ? null : _colorRepository.Get(prod.SecondColorId),
                        ThirdProductColorRecord = prod.ThirdColorId == 0 ? null : _colorRepository.Get(prod.ThirdColorId),
                        FourthProductColorRecord = prod.FourthColorId == 0 ? null : _colorRepository.Get(prod.FourthColorId),
                        FifthProductColorRecord = prod.FifthColorId == 0 ? null : _colorRepository.Get(prod.FifthColorId)
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
            return _campProdRepository.Table.Where(p => p.CampaignRecord_Id == campaignId).OrderBy(p => p.Id);
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
                                .Where(c => c.EndDate < DateTime.UtcNow && c.IsActive && c.IsApproved)
                                .ToList();

            Logger.Information("Check expired campaign --------------- > {0} expired campaigns found", campaigns.Count);

            foreach (var c in campaigns)
            {
                //c.CampaignStatusRecord = _statusRepository
                //                            .Table
                //                            .First(s => s.Name == CampaignStatus.Ended.ToString());

                c.IsActive = false;
                _campaignRepository.Update(c);
                _campaignRepository.Flush();

                if (!c.WhenDeleted.HasValue)
                {
                    var orders = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == c.Id && p.OrderRecord.IsActive).Select(pr => pr.OrderRecord).Distinct().ToList();

                    var isSuccesfull = c.ProductCountGoal <= c.ProductCountSold;
                    _teeyootMessagingService.SendExpiredCampaignMessageToSeller( c.Id, isSuccesfull);
                    _teeyootMessagingService.SendExpiredCampaignMessageToBuyers(c.Id, isSuccesfull);
                    _teeyootMessagingService.SendExpiredCampaignMessageToAdmin(c.Id, isSuccesfull);
                    
                    foreach (var o in orders)
                    {
                        if (o.OrderStatusRecord.Name == OrderStatus.Approved.ToString())
                        {
                            o.OrderStatusRecord = isSuccesfull ?
                                _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Printing.ToString()) :
                                _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Cancelled.ToString());

                            
                            if (isSuccesfull && o.TranzactionId != null)
                            {
                                try
                                {
                                    Gateway.Transaction.SubmitForSettlement(o.TranzactionId);
                                    o.Paid = DateTime.UtcNow;
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("Error when trying to make transaction ---------------------- > {0}", e.ToString());
                                }
                            }
                          
                            _orderRepository.Update(o);
                            _orderRepository.Flush();

                            string eventStr = isSuccesfull ?
                                T("The campaign successfully reached its goal!").ToString() :
                                T("The campaign failed to reach its goal by the deadline. You will not be charged and the shirts will not be printed.").ToString();

                            _orderHistoryRepository.Create(new OrderHistoryRecord
                            {
                                EventDate = DateTime.UtcNow,
                                OrderRecord_Id = o.Id,
                                Event = eventStr
                            });

                            eventStr = isSuccesfull ?
                                T("The campaign has ended and your order is now being printed!").ToString() :
                                T("Your order was cancelled.").ToString();

                            _orderHistoryRepository.Create(new OrderHistoryRecord
                            {
                                EventDate = DateTime.UtcNow,
                                OrderRecord_Id = o.Id,
                                Event = eventStr
                            });
                            _orderHistoryRepository.Flush();
                        }
                    }
                }
            }
        }


        public bool DeleteCampaign(int id)
        {
            try
            {
                var delCamp = _campaignRepository.Table.Where(c => c.Id == id).First();
                delCamp.WhenDeleted = DateTime.UtcNow;
                delCamp.IsActive = false;
                _campaignRepository.Update(delCamp);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public IQueryable<CampaignProductRecord> GetAllCampaignProducts()
        {
            return _campProdRepository.Table;
        }

        public bool PrivateCampaign(int id, bool change)
        {
            try
            {
                var camp = GetAllCampaigns().Where(c => c.Id == id).First();
                camp.IsPrivate = change;
                _campaignRepository.Update(camp);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetCampaignStatus(int id, CampaignStatus status)
        {
            var campaign = GetCampaignById(id);
            campaign.CampaignStatusRecord = _statusRepository.Table.First(s => s.Name == status.ToString());
            UpdateCampaign(campaign);
        }

        public void ReservCampaign(int id, string email)
        {
            var backCampaignRecord = new BringBackCampaignRecord();
            backCampaignRecord.Email = email;
            try
            {
                var campaign = GetCampaignById(id);
                backCampaignRecord.CampaignRecord = campaign;
                
                _backCampaignRepository.Create(backCampaignRecord);  
            }
             catch (Exception e)
             {
                   Logger.Error("Error when trying to make reservation of campaign ---------------------- > {0}", e.ToString());
             }
        }

        public int GetCountOfReservedRequestsOfCampaign(int id)
        {
           return _backCampaignRepository.Table.Where(c => c.CampaignRecord.Id == id).Count();
        }
    }
}