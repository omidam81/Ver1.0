using Ionic.Zip;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Teeyoot.FeaturedCampaigns.ViewModels;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminApproveCampaignsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ISiteService _siteService;
        private readonly IimageHelper _imageHelper;
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        public AdminApproveCampaignsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IimageHelper imageHelper, ITeeyootMessagingService teeyootMessagingService)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;
            _teeyootMessagingService = teeyootMessagingService;

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
                            .Where(c => !c.IsApproved)
                            .Count();

            var campaigns = (string.IsNullOrWhiteSpace(searchString) ?
                            _campaignService.GetAllCampaigns()
                                                                     :
                            _campaignService.GetAllCampaigns()
                            .Where(c => c.Title.Contains(searchString)))
                                .Where(c => !c.IsApproved)
                                .Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title,                                                   
                                                    Alias = c.Alias,
                                                    StartDate = c.StartDate.ToLocalTime()
                                                })
                                .Skip(pager.GetStartIndex())
                                .Take(pager.PageSize)
                                .ToList()
                                .OrderBy(e => e.Title);

            var entriesProjection = campaigns.Select(e =>
            {
                return Shape.campaign(
                    Id: e.Id,
                    Title: e.Title,
                    Alias: e.Alias,
                    StartDate: e.StartDate.ToLocalTime()
                    );
            });

            var pagerShape = Shape.Pager(pager).TotalItemCount(total);

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), SearchString = searchString, Pager = pagerShape, StartedIndex = pager.GetStartIndex() });
        }

        public ActionResult Approve(PagerParameters pagerParameters, string searchString, int id)
        {
            var campaign = _campaignService.GetCampaignById(id);
            campaign.IsApproved = true;
            _campaignService.UpdateCampaign(campaign);
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _teeyootMessagingService.SendLaunchCampaignMessage(pathToTemplates, pathToMedia, campaign.Id);
            return RedirectToAction("Index", new { PagerParameters=pagerParameters, SearchString=searchString });
        }

	}
}