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
    public class AdminExportPrintsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ISiteService _siteService;
        private readonly IimageHelper _imageHelper;

        public AdminExportPrintsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IimageHelper imageHelper)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;

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
                            //.Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold)
                            .Count();

            var campaigns = (string.IsNullOrWhiteSpace(searchString) ?
                            _campaignService.GetAllCampaigns()
                                                                     :
                            _campaignService.GetAllCampaigns()
                            .Where(c => c.Title.Contains(searchString)))
                                //.Where(c => !c.IsActive && c.ProductCountGoal <= c.ProductCountSold)
                                .Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title,
                                                    Sold = c.ProductCountSold,
                                                    Goal = c.ProductCountGoal,
                                                    Status = c.CampaignStatusRecord,
                                                    Alias = c.Alias
                                                })
                                .Skip(pager.GetStartIndex())
                                .Take(pager.PageSize)
                                .ToList()
                                .OrderBy(e => e.Status.Id);

            var entriesProjection = campaigns.Select(e =>
            {
                return Shape.campaign(
                    Id: e.Id,
                    Title: e.Title,
                    Sold: e.Sold,
                    Goal: e.Goal,
                    Status: e.Status,
                    Alias: e.Alias
                    );
            });

            var pagerShape = Shape.Pager(pager).TotalItemCount(total);

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), SearchString = searchString, Pager = pagerShape, StartedIndex = pager.GetStartIndex() });
        }

        public ActionResult ExportPrints(PagerParameters pagerParameters, string searchString, int id)
        {
            var campaign = _campaignService.GetCampaignById(id);

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var design = serializer.Deserialize<DesignInfo>(campaign.Design);

            var front = _imageHelper.Base64ToBitmap(design.Front);
            var back = _imageHelper.Base64ToBitmap(design.Back);

            var zipBytes = new byte[] { };

            using(var stream = new MemoryStream())
            {
                front.Save(stream, ImageFormat.Png);
                var frontBytes = stream.ToArray();
                stream.SetLength(0);
                back.Save(stream, ImageFormat.Png);
                var backBytes = stream.ToArray();
                stream.SetLength(0);

                using (var zip = new ZipFile())
                {
                    zip.AddEntry("front.png", frontBytes);
                    zip.AddEntry("back.png", backBytes);
                    zip.Save(stream);
                    zipBytes = stream.ToArray();
                }
            }

            return File(zipBytes, MediaTypeNames.Application.Zip, "campaign_" + id + "_" + campaign.Alias + "_" + "_prints.zip");
        }

        public ActionResult StartPrinting(PagerParameters pagerParameters, string searchString, int id)
        {
            _campaignService.SetCampaignStatus(id, CampaignStatus.Printing);
            return RedirectToAction("Index", new { pagerParameters, searchString });
        }
	}
}