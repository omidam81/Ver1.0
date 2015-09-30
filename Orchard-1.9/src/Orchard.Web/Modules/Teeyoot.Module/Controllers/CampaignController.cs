﻿using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using System;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using RM.Localization.Services;
using Orchard.Data;

using System.Linq;
using System.Collections.Generic;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class CampaignController : Controller
    {
        public IOrchardServices Services { get; set; }
        private readonly ICampaignService _campaignService;
        private readonly IPromotionService _promotionService;
        private readonly IWorkContextAccessor _wca;
        private readonly IProductService _productService;
        private readonly ITShirtCostService _tshirtService;
        private readonly INotifier _notifier;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        public Localizer T { get; set; }
        private readonly ICookieCultureService _cookieCultureService;
        private string cultureUsed = string.Empty;
        private readonly ICountryService _countryService;

        public CampaignController(ICampaignService campaignService, ITShirtCostService tshirtService, IProductService productService, IPromotionService promotionService, IRepository<CurrencyRecord> currencyRepository, IWorkContextAccessor wca, INotifier notifier, IOrchardServices services, ICookieCultureService cookieCultureService, ICountryService countryService)
        {
            _currencyRepository = currencyRepository;
            Services = services;
            _tshirtService = tshirtService;
            _productService = productService;
            _campaignService = campaignService;
            _promotionService = promotionService;
            _wca = wca;
            _notifier = notifier;
            Logger = NullLogger.Instance;

            _cookieCultureService = cookieCultureService;
            //var culture = _wca.GetContext().CurrentCulture.Trim();
            cultureUsed = _wca.GetContext().CurrentCulture.Trim();
            //cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            _countryService = countryService;
        }

        public ILogger Logger { get; set; }

        //
        // GET: /Campaign/
        public ActionResult Index(string campaignName, string promo)
        {
            if (!string.IsNullOrWhiteSpace(campaignName))
            {
                
                var campaign = _campaignService.GetCampaignByAlias(campaignName);

                if (campaign != null)
                {
                    var user = _wca.GetContext().CurrentUser;
                    int teeyootUserId = -1;

                    if (user != null)
                    {
                        teeyootUserId = user.ContentItem.Get(typeof(TeeyootUserPart)).Id;
                    }

                    if (campaign.IsApproved == true || Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId)
                    {
                        //TODO: (auth:keinlekan) Удалить код, если больше не пригодиться. Переход сайта на культуру компании
                        //string campaignCulture = campaign.CampaignCulture;
                        //if (campaignCulture != cultureUsed)
                        //{
                        //    _cookieCultureService.SetCulture(campaignCulture);
                        //}

                        if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId) && campaign.Rejected == true)
                        {
                            var infoMessage = T("Your campaign have been rejected!");
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                        else
                        {
                            if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId) && campaign.IsApproved == false)
                            {
                                var infoMessage = T("Your campaign is awaiting approval. This should take less than 1 hour during office hours.");
                                _notifier.Add(NotifyType.Information, infoMessage);
                            }
                        }

                        CampaignIndexViewModel model = new CampaignIndexViewModel() { };
                        model.Campaign = campaign;
                        model.FBDescription = model.Campaign.Description;
                        model.FBDescription = Regex.Replace(model.FBDescription, @"<br>", " ").Trim();
                        model.FBDescription = Regex.Replace(model.FBDescription, @"<[^>]+>", "").Trim();
                        model.FBDescription = Regex.Replace(model.FBDescription, @"&nbsp;", " ").Trim();
                        if (campaign.ProductCountSold >= campaign.ProductMinimumGoal && campaign.IsActive)
                        {
                            var infoMessage = T("Yippee! The minimum order for this campaign is {0}, but we have already sold {1}. The item will definitely go to print once the campaign ends.", campaign.ProductMinimumGoal, campaign.ProductCountSold);
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                        if (campaign.IsApproved == true && campaign.ProductCountSold < campaign.ProductMinimumGoal && campaign.IsActive)
                        {
                            var infoMessage = T(String.Format("{0} orders have been made. We need {1} more for this campaign to proceed.", campaign.ProductCountSold, campaign.ProductMinimumGoal - campaign.ProductCountSold));
                                _notifier.Add(NotifyType.Information, infoMessage);                          
                        }
                        if (!campaign.IsActive && campaign.IsApproved && !campaign.IsArchived)
                        {
                            var cntRequests =  _campaignService.GetCountOfReservedRequestsOfCampaign(campaign.Id);
                            model.CntRequests = 10 - (cntRequests >= 10 ? 10 : cntRequests);
                            if (cntRequests >= 10)
                            {
                                var infoMessage = T(String.Format("This campaign is likely to be re-activated soon."));
                                _notifier.Add(NotifyType.Information, infoMessage);
                            }
                            else
                            {
                                var infoMessage = T(String.Format("Only {0} more requests for the campaign to be re-activated", 10 - (cntRequests >= 10 ? 10 : cntRequests)));
                                _notifier.Add(NotifyType.Information, infoMessage);
                            }
                        }

                        if (promo != null)
                        {
                            try
                            {
                                PromotionRecord promotion = _promotionService.GetPromotionByPromoId(promo);
                                
                                                               
                                if (promotion.Status && (promotion.AmountType == _currencyRepository.Table.Where(c=> c.CurrencyCulture == campaign.CampaignCulture).FirstOrDefault().Code) && (promotion.Expiration > DateTime.UtcNow) && (campaign.ProductCountSold >= campaign.ProductMinimumGoal))
                                {
                                    var infoMessage = T(String.Format("Congratulations, you'll be receiving {0} {1} off your purchase. Discount reflected at checkout!", promotion.AmountType, promotion.AmountSize));
                                    //_notifier.Add(NotifyType.Error, infoMessage);
                                    model.PromoId = promo;
                                    model.PromoSize = promotion.AmountSize;
                                    model.PromoType = promotion.AmountType;
                                }
                                else
                                {
                                    var infoMessage = T("Sorry, this promo is expired!");
                                    _notifier.Add(NotifyType.Information, infoMessage);
                                }
                                return View(model);
                            }
                            catch (Exception)
                            {

                                var infoMessage = T("You have wrong promo code!");
                                _notifier.Add(NotifyType.Information, infoMessage);
                                return View(model);
                            }

                        }
                        return View(model);
                    }
                }
            }

            return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
        }

        [HttpGet]
        public JsonResult GetDataForReLaunch(string campaignName)
        {
            var campaign = _campaignService.GetCampaignByAlias(campaignName);
            var products = _campaignService.GetProductsOfCampaign(campaign.Id);
            var result = new RelaunchViewModel();
            List<object> prodInfo = new List<object>();
            foreach (var product in products)
            {
                var prodRec = _productService.GetProductById(product.ProductRecord.Id);
                prodInfo.Add(new { Price = product.Price, BaseCostForProduct = prodRec.BaseCost, ProductId = prodRec.Id, BaseCost = product.BaseCost });
            }

            var tShirtCostRecord = _tshirtService.GetCost(cultureUsed);

            result.Products = prodInfo.ToArray();
            result.CntBackColor = campaign.CntBackColor;
            result.CntFrontColor = campaign.CntFrontColor;
            result.TShirtCostRecord = tShirtCostRecord;
            result.ProductCountGoal = campaign.ProductCountSold;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


	}
}