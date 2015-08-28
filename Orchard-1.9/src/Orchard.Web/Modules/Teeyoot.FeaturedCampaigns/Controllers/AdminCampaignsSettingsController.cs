﻿using Ionic.Zip;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.Users.Models;
using System;
using System.Drawing;
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
            var totalNotApproved = _campaignService.GetAllCampaigns().Where(c => c.IsApproved == false && c.Rejected == false).Count();

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

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), NotApprovedTotal = totalNotApproved });
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
            
            var day = campaign.EndDate.Day;
            var mounth = campaign.EndDate.Month;
            var year = campaign.EndDate.Year;
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
        [ValidateInput(false)]
        public HttpStatusCodeResult SaveInfo(int campaignId, string Title, string URL, int Day, int Mounth, int Year, int Target, string Description, string Prices, string[] Colors)
        {
            var campaign = _campaignService.GetCampaignById(campaignId);
            var campaigns = _campaignService.GetAllCampaigns();
            if (!campaigns.Select(c=>c.Alias).ToList().Contains(URL) || campaign.Alias == URL)
            {
                DateTime date = new DateTime(Year, Mounth, Day);
                if (date < DateTime.Now)
                {
                    campaign.IsActive = false;
                    var isSuccesfull = campaign.ProductCountGoal <= campaign.ProductCountSold;
                    _teeyootMessagingService.SendExpiredCampaignMessageToSeller(campaign.Id, isSuccesfull);
                    _teeyootMessagingService.SendExpiredCampaignMessageToBuyers(campaign.Id, isSuccesfull);
                    _teeyootMessagingService.SendExpiredCampaignMessageToAdmin(campaign.Id, isSuccesfull);

                }
                else if (date > DateTime.Now)
                    campaign.IsActive = true;

                campaign.Title = Title;
                campaign.Alias = URL;
                campaign.ProductCountGoal = Target;
                campaign.Description = Description;
                campaign.EndDate = date.ToUniversalTime();
                var prices = Prices.Split(',');
                for (int i = 0; i < campaign.Products.Count; i++)
                    campaign.Products[i].Price = Convert.ToDouble(prices[i]);

                //for (int k = 0; k < Colors.Length; k++)
                //{
                //    var colors = Colors[k].Split('/').ToList();
                //    int prodId = Int32.Parse(colors[0]);
                //    colors.RemoveAt(0);

                //    var prod = campaign.Products.Where(c => c.Id == prodId);
                    
                //    foreach(var col in colors){
                //        if (col == null || Int32.Parse(col) == 0)
                //        {
                //            colors.Remove(col);
                //        }
                //    }
                //}

                    _campaignService.UpdateCampaign(campaign);
                var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                _teeyootMessagingService.SendEditedCampaignMessageToSeller(campaign.Id, pathToMedia, pathToTemplates);
                Response.Write(true);
            
            }
            else {
                Response.Write(false);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public void CreateImagesForOtherColor(int campaignId, string prodIdAndColor, CampaignProductRecord p, DesignInfo data, string frontPath, string backPath, string color)
        {
            var destForder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaignId.ToString(), prodIdAndColor);

            if (!Directory.Exists(destForder))
            {
                Directory.CreateDirectory(destForder + "/normal");
                Directory.CreateDirectory(destForder + "/big");
            }

            var frontTemplate = new Bitmap(frontPath);
            var backTemplate = new Bitmap(backPath);

            var rgba = ColorTranslator.FromHtml(color);

            var front = BuildProductImage(frontTemplate, _imageHelper.Base64ToBitmap(data.Front), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                p.ProductRecord.ProductImageRecord.PrintableFrontTop, p.ProductRecord.ProductImageRecord.PrintableFrontLeft,
                p.ProductRecord.ProductImageRecord.PrintableFrontWidth, p.ProductRecord.ProductImageRecord.PrintableFrontHeight);
            front.Save(Path.Combine(destForder, "normal", "front.png"));

            var back = BuildProductImage(backTemplate, _imageHelper.Base64ToBitmap(data.Back), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                p.ProductRecord.ProductImageRecord.PrintableBackTop, p.ProductRecord.ProductImageRecord.PrintableBackLeft,
                p.ProductRecord.ProductImageRecord.PrintableBackWidth, p.ProductRecord.ProductImageRecord.PrintableBackHeight);
            back.Save(Path.Combine(destForder, "normal", "back.png"));

            _imageHelper.ResizeImage(front, 1070, 1274).Save(Path.Combine(destForder, "big", "front.png"));
            _imageHelper.ResizeImage(back, 1070, 1274).Save(Path.Combine(destForder, "big", "back.png"));

            frontTemplate.Dispose();
            backTemplate.Dispose();
            front.Dispose();
            back.Dispose();
        }

        private Bitmap BuildProductImage(Bitmap image, Bitmap design, Color color, int width, int height, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight)
        {
            var background = _imageHelper.CreateBackground(width, height, color);
            image = _imageHelper.ApplyBackground(image, background, width, height);
            return _imageHelper.ApplyDesign(image, design, printableAreaTop, printableAreaLeft, printableAreaWidth, printableAreaHeight, width, height);
        }

	}
}