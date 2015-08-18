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
using System.Net;
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
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IOrderService _orderService;

        public AdminCampaignsSettingsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IimageHelper imageHelper, IOrderService orderService, IContentManager contentManager,
            ITeeyootMessagingService teeyootMessagingService)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;
            _contentManager = contentManager;
            _teeyootMessagingService = teeyootMessagingService;
            _orderService = orderService;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {          
            var total = _campaignService.GetAllCampaigns().Count();

            var campaigns = _campaignService.GetAllCampaigns().Select(c => new { 
                                                    Id = c.Id,
                                                    Title = c.Title, 
                                                    Goal = c.ProductCountGoal,
                                                    Sold = c.ProductCountSold,
                                                    Status = c.CampaignStatusRecord.Name,
                                                    UserId = c.TeeyootUserId,
                                                    IsApproved = c.IsApproved,
                                                    EndDate = c.EndDate,
                                                    Profit = c.CampaignProfit,
                                                    Alias =c.Alias,
                                                    IsActive = c.IsActive,
                                                    Minimum = c.ProductMinimumGoal,
                                                    CreateDate=c.StartDate
                                                })                               
                                .ToList()
                                .OrderBy(e => e.Title);
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday);

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
                    EndDate: e.EndDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    Profit : e.Profit,
                    Alias : e.Alias,
                    IsActive: e.IsActive,
                    Minimum: e.Minimum,
                    CreatedDate: e.CreateDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    Last24HoursSold : last24hoursOrders
                                        .SelectMany(o => o.Products)
                                        .Where(p => p.CampaignProductRecord.CampaignRecord_Id == e.Id)
                                        .Sum(p => (int?)p.Count) ?? 0
                    );
            });
          
            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray() });
        }

        public ActionResult ChangeStatus(PagerParameters pagerParameters, int id, CampaignStatus status)
        { 
            _campaignService.SetCampaignStatus(id, status);
            _teeyootMessagingService.SendChangedCampaignStatusMessage(id, status.ToString()); 
            return RedirectToAction("Index", new { PagerParameters=pagerParameters });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult ChangeEndDate(int campaignId, int day, int month, int year )
        {
            DateTime date = new DateTime(year,month, day);
            var campaign = _campaignService.GetCampaignById(campaignId);
            campaign.EndDate = date.ToUniversalTime();

            Response.Write(campaign.EndDate.ToLocalTime().ToString("dd/MM/yyyy"));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult ChangeInformation(int Id) {
            var campaign = _campaignService.GetCampaignById(Id);
            var day = campaign.EndDate.ToString().Split('.')[0];
            var mounth = campaign.EndDate.ToString().Split('.')[1];
            var year = campaign.EndDate.ToString().Split('.')[2].Substring(0,4);
            var model = new CampaignInfViewModel()
            {
                CampaignId = campaign.Id,
                Title = campaign.Title,
                Alias = campaign.Alias,
                Target = campaign.ProductCountGoal,
                Day = Convert.ToInt32(day),
                Mounth = Convert.ToInt32(mounth),
                Year = Convert.ToInt32(year),
                Description = campaign.Description,
                Products = campaign.Products
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult SaveInfo(int campaignId, string Title, string URL, int Day, int Mounth, int Year, int Target, string Description, string Prices)
        {
            var campaign = _campaignService.GetCampaignById(campaignId);
            var campaigns = _campaignService.GetAllCampaigns();
            if (!campaigns.Select(c=>c.Alias).ToList().Contains(URL) || campaign.Alias == URL)
            {
                DateTime date = new DateTime(Year, Mounth, Day);

                campaign.Title = Title;
                campaign.Alias = URL;
                campaign.ProductCountGoal = Target;
                //campaign.Description = Description;
                campaign.EndDate = date.ToUniversalTime();
                var prices = Prices.Split(',');
                for (int i = 0; i < campaign.Products.Count; i++)
                    campaign.Products[i].Price = Convert.ToDouble(prices[i]);
                _campaignService.UpdateCampaign(campaign);
                Response.Write(true);
            }
            else {
                Response.Write(false);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

	}
}