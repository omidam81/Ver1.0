using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FeaturedCampaigns.ViewModels;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Services;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminExportPrintsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ISiteService _siteService;

        public AdminExportPrintsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory )
        {
            _campaignService = campaignService;
            _siteService = siteService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
                 
            var total = _campaignService.GetAllCampaigns()
                                .Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold && c.CampaignStatusRecord.Name != CampaignStatus.Printing.ToString())
                                .Count();

            var campaigns = _campaignService.GetAllCampaigns()
                                .Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold && c.CampaignStatusRecord.Name != CampaignStatus.Printing.ToString())
                                .Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    Sold = c.ProductCountSold,
                                                    Goal = c.ProductCountGoal
                                                })
                                .Skip(pager.GetStartIndex())
                                .Take(pager.PageSize)
                                .ToList();

            var entriesProjection = campaigns.Select(e =>
            {
                return Shape.campaign(
                    Id: e.Id,
                    Title: e.Title,
                    Sold: e.Sold,
                    Goal: e.Goal
                    );
            });

            var pagerShape = Shape.Pager(pager).TotalItemCount(total);

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), Pager = pagerShape, StartedIndex = pager.GetStartIndex() });
        }

        public ActionResult ExportPrints()
        {
            return RedirectToAction("Index");
        }
	}
}