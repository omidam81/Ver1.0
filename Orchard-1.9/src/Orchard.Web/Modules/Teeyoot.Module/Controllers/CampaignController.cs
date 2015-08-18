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

        public CampaignController(ICampaignService campaignService, IPromotionService promotionService, IWorkContextAccessor wca, INotifier notifier, IOrchardServices services)
        {
            Services = services;
            _campaignService = campaignService;
            _promotionService = promotionService;
            _wca = wca;
            _notifier = notifier;
            Logger = NullLogger.Instance;
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

                        if (campaign.ProductCountSold >= campaign.ProductMinimumGoal)
                        {
                            var infoMessage = T("The minimum order has been reached, so this shirt will definitely go to print.");
                            _notifier.Add(NotifyType.Information, infoMessage);
                        }
                        if (campaign.IsApproved == true && campaign.ProductCountSold <= campaign.ProductMinimumGoal)
                        {                           
                                var infoMessage = T(String.Format("The minimum number of orders for this campaign to be printed is {0}", campaign.ProductMinimumGoal - campaign.ProductCountSold));
                                _notifier.Add(NotifyType.Information, infoMessage);                          
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