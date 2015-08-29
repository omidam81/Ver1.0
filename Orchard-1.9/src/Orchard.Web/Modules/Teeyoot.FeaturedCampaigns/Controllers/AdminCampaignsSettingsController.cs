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
using System.Collections.Generic;
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

            var campaigns = _campaignService.GetAllCampaigns().Select(c => new
            {
                Id = c.Id,
                Title = c.Title,
                Goal = c.ProductCountGoal,
                Sold = c.ProductCountSold,
                Status = c.CampaignStatusRecord.Name,
                UserId = c.TeeyootUserId,
                IsApproved = c.IsApproved,
                EndDate = c.EndDate,
                Profit = c.CampaignProfit,
                Alias = c.Alias,
                IsActive = c.IsActive,
                Minimum = c.ProductMinimumGoal,
                CreateDate = c.StartDate
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
                    Sold: e.Sold,
                    Goal: e.Goal,
                    Seller: _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == e.UserId),
                    TeeyootSeller: _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == e.UserId).ContentItem.Get(typeof(TeeyootUserPart)),
                    IsApproved: e.IsApproved,
                    EndDate: e.EndDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    Profit: e.Profit,
                    Alias: e.Alias,
                    IsActive: e.IsActive,
                    Minimum: e.Minimum,
                    CreatedDate: e.CreateDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    Last24HoursSold: last24hoursOrders
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
            return RedirectToAction("Index", new { PagerParameters = pagerParameters });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult ChangeEndDate(int campaignId, int day, int month, int year)
        {
            DateTime date = new DateTime(year, month, day);
            var campaign = _campaignService.GetCampaignById(campaignId);
            campaign.EndDate = date.ToUniversalTime();

            Response.Write(campaign.EndDate.ToLocalTime().ToString("dd/MM/yyyy"));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult ChangeInformation(int Id)
        {
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
            if (!campaigns.Select(c => c.Alias).ToList().Contains(URL) || campaign.Alias == URL)
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

                for (int k = 0; k < Colors.Length; k++)
                {
                    var colors = Colors[k].Split('/').ToList();
                    int prodId = Int32.Parse(colors[0]);
                    colors.RemoveAt(0);

                    var prod = campaign.Products.Where(c => c.Id == prodId).First();

                    List<int> newCol = new List<int>();
                    foreach (var col in colors)
                    {
                        if (col == null || string.IsNullOrEmpty(col) || Int32.Parse(col) == 0)
                        {
                            //colors.Remove(col);
                        }
                        else
                        {
                            newCol.Add(Int32.Parse(col));
                        }
                    }

                    //List<int> nowIds = new List<int>{ prod.ProductColorRecord.Id,
                    //                   prod.SecondProductColorRecord != null ? prod.SecondProductColorRecord.Id : 0,
                    //                   prod.ThirdProductColorRecord != null ? prod.ThirdProductColorRecord.Id : 0,
                    //                   prod.FourthProductColorRecord != null ? prod.FourthProductColorRecord.Id : 0,
                    //                   prod.FifthProductColorRecord != null ? prod.FifthProductColorRecord.Id : 0};

                    List<bool> noIdsBool = new List<bool> { false, false, false, false, false };


                    List<int> masResult = new List<int>();
                    foreach (var col in newCol)
                    {
                        if (prod.ProductColorRecord.Id == col)
                        {
                            //colors.Remove(col);
                            noIdsBool[0] = true;
                            continue;
                        }

                        if (prod.SecondProductColorRecord.Id == col)
                        {
                            //colors.Remove(col);
                            noIdsBool[1] = true;
                            continue;
                        }

                        if (prod.ThirdProductColorRecord.Id == col)
                        {
                            //colors.Remove(col);
                            noIdsBool[2] = true;
                            continue;
                        }

                        if (prod.FourthProductColorRecord.Id == col)
                        {
                            //colors.Remove(col);
                            noIdsBool[3] = true;
                            continue;
                        }

                        if (prod.FifthProductColorRecord.Id == col)
                        {
                            //colors.Remove(col);
                            noIdsBool[4] = true;
                            continue;
                        }
                        masResult.Add(col);
                    }

                    if (masResult != null && masResult.Count != 0)
                    {
                        foreach (var col in masResult)
                        {
                            var serializer = new JavaScriptSerializer();
                            serializer.MaxJsonLength = int.MaxValue;
                            var data = serializer.Deserialize<DesignInfo>(campaign.Design);

                            var color = prod.ProductRecord.ColorsAvailable.Where(c => c.ProductColorRecord.Id == col).First().ProductColorRecord;
                            if (!noIdsBool.ElementAt(0))
                            {
                                if (prod.ProductColorRecord != null)
                                {
                                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id.ToString()));
                                    try
                                    {
                                        dir.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message);
                                    }
                                }

                                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                                var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                                var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                                CreateImagesForOtherColor(campaign.Id, prod.Id.ToString(), prod, data, frontPath, backPath, color.Value);

                                prod.ProductColorRecord = color;
                                continue;
                            }

                            if (!noIdsBool.ElementAt(1))
                            {
                                if (prod.SecondProductColorRecord != null)
                                {
                                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.SecondProductColorRecord.Id.ToString()));
                                    try
                                    {
                                        dir.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message);
                                    }
                                }

                                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                                var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                                var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                                CreateImagesForOtherColor(campaign.Id, prod.Id.ToString() + "_" + color.Id.ToString(), prod, data, frontPath, backPath, color.Value);

                                prod.ProductColorRecord = color;
                                continue;
                            }

                            if (!noIdsBool.ElementAt(2))
                            {
                                if (prod.ThirdProductColorRecord != null)
                                {
                                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.ThirdProductColorRecord.Id.ToString()));
                                    try
                                    {
                                        dir.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message);
                                    }
                                }

                                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                                var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                                var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                                CreateImagesForOtherColor(campaign.Id, prod.Id.ToString() + "_" + color.Id.ToString(), prod, data, frontPath, backPath, color.Value);

                                prod.ProductColorRecord = color;
                                continue;
                            }

                            if (!noIdsBool.ElementAt(3))
                            {
                                if (prod.FourthProductColorRecord != null)
                                {
                                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.FourthProductColorRecord.Id.ToString()));
                                    try
                                    {
                                        dir.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message);
                                    }
                                }

                                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                                var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                                var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                                CreateImagesForOtherColor(campaign.Id, prod.Id.ToString() + "_" + color.Id.ToString(), prod, data, frontPath, backPath, color.Value);

                                prod.ProductColorRecord = color;
                                continue;
                            }

                            if (!noIdsBool.ElementAt(4))
                            {
                                if (prod.FifthProductColorRecord != null)
                                {
                                    DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.FifthProductColorRecord.Id.ToString()));
                                    try
                                    {
                                        dir.Delete(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex.Message);
                                    }
                                }

                                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                                var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                                var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                                CreateImagesForOtherColor(campaign.Id, prod.Id.ToString() + "_" + color.Id.ToString(), prod, data, frontPath, backPath, color.Value);

                                prod.ProductColorRecord = color;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (!noIdsBool[1])
                        {
                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.SecondProductColorRecord.Id.ToString()));
                            try
                            {
                                dir.Delete(true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                            prod.SecondProductColorRecord = null;
                        }
                        if (!noIdsBool[2])
                        {
                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.ThirdProductColorRecord.Id.ToString()));
                            try
                            {
                                dir.Delete(true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                            prod.ThirdProductColorRecord = null;
                        }
                        if (!noIdsBool[3])
                        {
                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.FourthProductColorRecord.Id.ToString()));
                            try
                            {
                                dir.Delete(true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                            prod.FourthProductColorRecord = null;
                        }
                        if (!noIdsBool[4])
                        {
                            DirectoryInfo dir = new DirectoryInfo(Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id + "_" + prod.FifthProductColorRecord.Id.ToString()));
                            try
                            {
                                dir.Delete(true);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex.Message);
                            }
                            prod.FifthProductColorRecord = null;
                        }
                    }
                }

                _campaignService.UpdateCampaign(campaign);
                var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                _teeyootMessagingService.SendEditedCampaignMessageToSeller(campaign.Id, pathToMedia, pathToTemplates);
                Response.Write(true);

            }
            else
            {
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