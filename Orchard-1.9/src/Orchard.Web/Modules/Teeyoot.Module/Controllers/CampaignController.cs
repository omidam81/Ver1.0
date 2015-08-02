using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly IPromotionService _promotionService;

        public CampaignController(ICampaignService campaignService, IPromotionService promotionService)
        {
            _campaignService = campaignService;
            _promotionService = promotionService;
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
                    CampaignIndexViewModel model = new CampaignIndexViewModel(){};
                    model.Campaign = campaign;
                    if (promo != null)
                    {
                        PromotionRecord promotion = _promotionService.GetPromotionByPromoId(promo);
                        string infomessage = String.Format("Congratulations, you'll be receiving {0}{1} off your purchase. Discount reflected at checkout!", promotion.AmountSize, promotion.AmountType);
                        model.InfoMessage = infomessage;
                        model.PromoId = promo;
                    }
                    return View(model);
                }
            }

            return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
        }
	}
}