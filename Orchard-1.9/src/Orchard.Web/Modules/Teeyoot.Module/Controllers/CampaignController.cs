using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        //
        // GET: /Campaign/
        public ActionResult Index(string campaignName)
        {
            if (!string.IsNullOrWhiteSpace(campaignName))
            {
                var campaign = _campaignService.GetCampaignByAlias(campaignName);

                if (campaign != null)
                {
                    return View(campaign);
                }
            }
           
            return new EmptyResult();
        }
	}
}