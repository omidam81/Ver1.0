using Ionic.Zip;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.Users.Models;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Teeyoot.FeaturedCampaigns.ViewModels;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminCampaignsSettingsController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ISiteService _siteService;
        private readonly IimageHelper _imageHelper;
        private readonly IContentManager _contentManager;

        public AdminCampaignsSettingsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IimageHelper imageHelper,  IContentManager contentManager)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;
            _contentManager = contentManager;

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
                            .Count();

            var campaigns = (string.IsNullOrWhiteSpace(searchString) ?
                            _campaignService.GetAllCampaigns()
                                                                     :
                            _campaignService.GetAllCampaigns()
                            .Where(c => c.Title.Contains(searchString)))                                
                                .Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title, 
                                                    Goal = c.ProductCountGoal,
                                                    Sold = c.ProductCountSold,
                                                    Status = c.CampaignStatusRecord.Name,
                                                    UserId = c.TeeyootUserId,
                                                    IsApproved = c.IsApproved,
                                                    EndDate = c.EndDate
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
                    Status: e.Status,
                    Sold : e.Sold,
                    Goal : e.Goal,
                    Seller: _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == e.UserId),
                    TeeyootSeller: _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == e.UserId).ContentItem.Get(typeof(TeeyootUserPart)),
                    IsApproved: e.IsApproved,
                    EndDate : e.EndDate
                    );
            });

            var pagerShape = Shape.Pager(pager).TotalItemCount(total);

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), SearchString = searchString, Pager = pagerShape, StartedIndex = pager.GetStartIndex() });
        }

        public ActionResult ChangeStatus(PagerParameters pagerParameters, string searchString, int id, CampaignStatus status)
        { 
            _campaignService.SetCampaignStatus(id, status);
            
            return RedirectToAction("Index", new { PagerParameters=pagerParameters, SearchString=searchString });
        }

	}
}