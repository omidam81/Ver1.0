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

        public ActionResult Index(PagerParameters pagerParameters, string searchString)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var total = (string.IsNullOrWhiteSpace(searchString) ?
                        _campaignService.GetAllCampaigns()                              
                                                                 :
                        _campaignService.GetAllCampaigns()
                        .Where(c => c.Title.Contains(searchString)))
                            .Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold)
                            .Count();

            var campaigns = (string.IsNullOrWhiteSpace(searchString) ?
                            _campaignService.GetAllCampaigns()
                                                                     :
                            _campaignService.GetAllCampaigns()
                            .Where(c => c.Title.Contains(searchString)))
                                .Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold)
                                .Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    Sold = c.ProductCountSold,
                                                    Goal = c.ProductCountGoal,
                                                    Status = c.CampaignStatusRecord
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
                    Goal: e.Goal,
                    Status: e.Status
                    );
            });

            var pagerShape = Shape.Pager(pager).TotalItemCount(total);

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), SearchString = searchString, Pager = pagerShape, StartedIndex = pager.GetStartIndex() });
        }

        public ActionResult ExportPrints(PagerParameters pagerParameters, string searchString, int id)
        {
            return RedirectToAction("Index", new { pagerParameters, searchString });
        }

        public ActionResult StartPrinting(PagerParameters pagerParameters, string searchString, int id)
        {
            _campaignService.SetCampaignStatus(id, CampaignStatus.Printing);
            return RedirectToAction("Index", new { pagerParameters, searchString });
        }
	}
}