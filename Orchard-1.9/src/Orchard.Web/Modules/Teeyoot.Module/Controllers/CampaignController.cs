using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using RM.Localization.Services;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Localization;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;

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
        private readonly string _cultureUsed;
        private readonly ICountryService _countryService;

        public CampaignController(
            ICampaignService campaignService,
            ITShirtCostService tshirtService,
            IProductService productService,
            IPromotionService promotionService,
            IRepository<CurrencyRecord> currencyRepository,
            IWorkContextAccessor wca,
            INotifier notifier,
            IOrchardServices services,
            ICookieCultureService cookieCultureService,
            ICountryService countryService)
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
            _cultureUsed = _wca.GetContext().CurrentCulture.Trim();
            //cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            _countryService = countryService;
        }

        public ILogger Logger { get; set; }

        //
        // GET: /Campaign/
        public ActionResult Index(string campaignName, string promo)
        {
            if (string.IsNullOrWhiteSpace(campaignName))
            {
                return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
            }

            var campaign = _campaignService.GetCampaignByAlias(campaignName);

            if (campaign == null)
            {
                return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
            }

            var user = _wca.GetContext().CurrentUser;
            var teeyootUserId = -1;

            if (user != null)
            {
                teeyootUserId = user.ContentItem.Get(typeof (TeeyootUserPart)).Id;
            }

            if (!campaign.IsApproved &&
                !Services.Authorizer.Authorize(Permissions.ApproveCampaigns) &&
                teeyootUserId != campaign.TeeyootUserId)
            {
                return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
            }

            //TODO: (auth:keinlekan) Удалить код, если больше не пригодиться. Переход сайта на культуру компании
            //string campaignCulture = campaign.CampaignCulture;
            //if (campaignCulture != cultureUsed)
            //{
            //    _cookieCultureService.SetCulture(campaignCulture);
            //}

            if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId) &&
                campaign.Rejected)
            {
                var infoMessage = T("Your campaign have been rejected!");
                _notifier.Add(NotifyType.Information, infoMessage);
            }
            else
            {
                if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) ||
                     teeyootUserId == campaign.TeeyootUserId) && campaign.IsApproved == false)
                {
                    var infoMessage =
                        T(
                            "Your campaign is awaiting approval. This should take less than 1 hour during office hours.");
                    _notifier.Add(NotifyType.Information, infoMessage);
                }
            }

            var model = new CampaignIndexViewModel {Campaign = campaign};
            model.FBDescription = model.Campaign.Description;
            model.FBDescription = Regex.Replace(model.FBDescription, @"<br>", " ").Trim();
            model.FBDescription = Regex.Replace(model.FBDescription, @"<[^>]+>", "").Trim();
            model.FBDescription = Regex.Replace(model.FBDescription, @"&nbsp;", " ").Trim();
            if (campaign.ProductCountSold >= campaign.ProductMinimumGoal && campaign.IsActive)
            {
                var infoMessage =
                    T(
                        "Yippee! The minimum order for this campaign is {0}, but we have already sold {1}. The item will definitely go to print once the campaign ends.",
                        campaign.ProductMinimumGoal, campaign.ProductCountSold);
                _notifier.Add(NotifyType.Information, infoMessage);
            }
            if (campaign.IsApproved && campaign.ProductCountSold < campaign.ProductMinimumGoal &&
                campaign.IsActive)
            {
                var infoMessage = T(
                    string.Format(
                        "{0} orders have been made. We need {1} more for this campaign to proceed.",
                        campaign.ProductCountSold,
                        campaign.ProductMinimumGoal - campaign.ProductCountSold));
                _notifier.Add(NotifyType.Information, infoMessage);
            }
            if (!campaign.IsActive && campaign.IsApproved && !campaign.IsArchived)
            {
                var cntRequests = _campaignService.GetCountOfReservedRequestsOfCampaign(campaign.Id);
                model.CntRequests = 10 - (cntRequests >= 10 ? 10 : cntRequests);
                if (cntRequests >= 10)
                {
                    var infoMessage = T("This campaign is likely to be re-activated soon.");
                    _notifier.Add(NotifyType.Information, infoMessage);
                }
                else
                {
                    var infoMessage = T(
                        string.Format("Only {0} more requests for the campaign to be re-activated",
                            10 - (cntRequests >= 10 ? 10 : cntRequests)));
                    _notifier.Add(NotifyType.Information, infoMessage);
                }
            }

            if (promo == null)
            {
                return View(model);
            }

            try
            {
                var promotion = _promotionService.GetPromotionByPromoId(promo);

                var localizationInfo = LocalizationInfoFactory.GetCurrentLocalizationInfo();
                var currency = _countryService.GetCurrency(localizationInfo);

                if (promotion.Status &&
                    promotion.Expiration > DateTime.UtcNow &&
                    promotion.UserId == campaign.TeeyootUserId &&
                    campaign.ProductCountSold >= campaign.ProductMinimumGoal)
                {
                    if (promotion.AmountType == "%")
                    {
                        FillViewModelWithPromo(model, promotion);
                    }
                    else
                    {
                        var promotionCurrency = _currencyRepository.Table
                            .First(c => c.Code == promotion.AmountType);

                        if (promotionCurrency == currency)
                        {
                            FillViewModelWithPromo(model, promotion);
                        }
                        else
                        {
                            var infoMessage = T(
                                "Oh no! The requested promotion is currently not available for this campaign. But you can still buy at the normal price!");
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                    }
                }
                else
                {
                    var infoMessage = T(
                        "Oh no! The requested promotion is currently not available for this campaign. But you can still buy at the normal price!");
                    _notifier.Add(NotifyType.Information, infoMessage);
                }

                return View(model);
            }
            catch (Exception)
            {

                var infoMessage = T(
                    "Oh no! The requested promotion is currently not available for this campaign. But you can still buy at the normal price!");
                _notifier.Add(NotifyType.Information, infoMessage);

                return View(model);
            }
        }

        [HttpGet]
        public JsonResult GetDataForReLaunch(string campaignName)
        {
            var campaign = _campaignService.GetCampaignByAlias(campaignName);
            var products = _campaignService.GetProductsOfCampaign(campaign.Id);
            var result = new RelaunchViewModel();
            var prodInfo = new List<object>();
            foreach (var product in products)
            {
                var prodRec = _productService.GetProductById(product.ProductRecord.Id);
                prodInfo.Add(new
                {
                    Price = product.Price,
                    BaseCostForProduct = prodRec.BaseCost,
                    ProductId = prodRec.Id,
                    BaseCost = product.BaseCost
                });
            }

            var tShirtCostRecord = _tshirtService.GetCost(_cultureUsed);

            result.Products = prodInfo.ToArray();
            result.CntBackColor = campaign.CntBackColor;
            result.CntFrontColor = campaign.CntFrontColor;
            result.TShirtCostRecord = tShirtCostRecord;
            result.ProductCountGoal = campaign.ProductCountSold;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static void FillViewModelWithPromo(CampaignIndexViewModel viewModel, PromotionRecord promotion)
        {
            viewModel.PromoId = promotion.PromoId;
            viewModel.PromoSize = promotion.AmountSize;
            viewModel.PromoType = promotion.AmountType;
        }
    }
}