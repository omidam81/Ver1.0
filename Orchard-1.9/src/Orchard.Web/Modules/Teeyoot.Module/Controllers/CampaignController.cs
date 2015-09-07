using Orchard;
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

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class CampaignController : Controller
    {
        public IOrchardServices Services { get; set; }
        private readonly ICampaignService _campaignService;
        private readonly IPromotionService _promotionService;
        private readonly IWorkContextAccessor _wca;
        private readonly INotifier _notifier;
        public Localizer T { get; set; }
        private readonly ICookieCultureService _cookieCultureService;
        private string cultureUsed = string.Empty;

        public CampaignController(ICampaignService campaignService, IPromotionService promotionService, IWorkContextAccessor wca, INotifier notifier, IOrchardServices services, ICookieCultureService cookieCultureService)
        {
            Services = services;
            _campaignService = campaignService;
            _promotionService = promotionService;
            _wca = wca;
            _notifier = notifier;
            Logger = NullLogger.Instance;

            _cookieCultureService = cookieCultureService;
            var culture = _wca.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
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
                        string campaignCulture = campaign.CampaignCulture;
                        if (campaignCulture != cultureUsed)
                        {
                            _cookieCultureService.SetCulture(campaignCulture);
                        }

                        if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId) && campaign.Rejected == true)
                        {
                            var infoMessage = T("Your campaign have been rejected!");
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                        else
                        {
                            if ((Services.Authorizer.Authorize(Permissions.ApproveCampaigns) || teeyootUserId == campaign.TeeyootUserId) && campaign.IsApproved == false)
                            {
                                var infoMessage = T("Your design has been sent, wait for confirmation.");
                                _notifier.Add(NotifyType.Information, infoMessage);
                            }
                        }

                        CampaignIndexViewModel model = new CampaignIndexViewModel() { };
                        model.Campaign = campaign;

                        if (campaign.ProductCountSold >= campaign.ProductMinimumGoal && campaign.IsActive)
                        {
                            var infoMessage = T("The minimum order has been reached, so this shirt will definitely go to print.");
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                        if (campaign.IsApproved == true && campaign.ProductCountSold < campaign.ProductMinimumGoal && campaign.IsActive)
                        {
                            var infoMessage = T(String.Format("{0} orders have been made. We need {1} more for this campaign to proceed.", campaign.ProductCountSold, campaign.ProductMinimumGoal - campaign.ProductCountSold));
                                _notifier.Add(NotifyType.Information, infoMessage);                          
                        }
                        if (!campaign.IsActive && campaign.IsApproved)
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
                                if (promotion.Status)
                                {
                                    var infoMessage = T(String.Format("Congratulations, you'll be receiving {0}{1} off your purchase. Discount reflected at checkout!", promotion.AmountSize, promotion.AmountType));
                                    _notifier.Add(NotifyType.Information, infoMessage);
                                    model.PromoId = promo;
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
	}
}