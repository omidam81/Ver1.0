using Ionic.Zip;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
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
        private readonly INotifier _notifier;
        private readonly IRepository<ProductColorRecord> _repositoryColor;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        public AdminCampaignsSettingsController(ICampaignService campaignService, ISiteService siteService, IShapeFactory shapeFactory, IimageHelper imageHelper, IOrderService orderService, IContentManager contentManager,
            ITeeyootMessagingService teeyootMessagingService, INotifier notifier, IRepository<ProductColorRecord> repositoryColor, IWorkContextAccessor workContextAccessor)
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
            _notifier = notifier;
            _repositoryColor = repositoryColor;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var total = _campaignService.GetAllCampaigns().Where(c => c.CampaignCulture == cultureUsed).Count();

            var campaigns = _campaignService.GetAllCampaigns().Where(c => c.CampaignCulture == cultureUsed).Select(c => new
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
            var totalNotApproved = _campaignService.GetAllCampaigns().Where(c => c.IsApproved == false && c.Rejected == false && c.CampaignCulture == cultureUsed).Count();

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday && o.CurrencyRecord.CurrencyCulture == cultureUsed);

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
                Products = campaign.Products.Where(c => c.WhenDeleted == null)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public HttpStatusCodeResult SaveInfo(int campaignId, string Title, string URL, int Day, int Mounth, int Year, int Target, string Description, string Prices, string[] Colors)
        {
            var campaign = _campaignService.GetCampaignById(campaignId);
            var campaigns = _campaignService.GetAllCampaigns();

            bool resultError = false;

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
                var prods = campaign.Products.Where(c => c.WhenDeleted == null).ToList();
                for (int i = 0; i < prods.Count; i++)
                    prods[i].Price = Convert.ToDouble(prices[i]);

                for (int k = 0; k < Colors.Length; k++)
                {
                    var colors = Colors[k].Split('/').ToList();
                    int prodId = Int32.Parse(colors[0]);
                    colors.RemoveAt(0);

                    var prod = campaign.Products.Where(c => c.Id == prodId).First();

                    string productPath1 = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), prod.Id.ToString());
                    string productPath2 = prod.SecondProductColorRecord != null ? Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), string.Format("{0}_{1}", prod.Id.ToString(), prod.SecondProductColorRecord.Id.ToString())) : string.Empty;
                    string productPath3 = prod.ThirdProductColorRecord != null ? Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), string.Format("{0}_{1}", prod.Id.ToString(), prod.ThirdProductColorRecord.Id.ToString())) : string.Empty;
                    string productPath4 = prod.FourthProductColorRecord != null ? Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), string.Format("{0}_{1}", prod.Id.ToString(), prod.FourthProductColorRecord.Id.ToString())) : string.Empty;
                    string productPath5 = prod.FifthProductColorRecord != null ? Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), string.Format("{0}_{1}", prod.Id.ToString(), prod.FifthProductColorRecord.Id.ToString())) : string.Empty;

                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(productPath1);
                        if (dir.Exists) dir.Delete(true);
                        if (!string.IsNullOrEmpty(productPath2))
                        {
                            DirectoryInfo dir2 = new DirectoryInfo(productPath2);
                            if (dir2.Exists) dir2.Delete(true);
                        }
                        if (!string.IsNullOrEmpty(productPath3))
                        {
                            DirectoryInfo dir3 = new DirectoryInfo(productPath3);
                            if (dir3.Exists) dir3.Delete(true);
                        }
                        if (!string.IsNullOrEmpty(productPath4))
                        {
                            DirectoryInfo dir4 = new DirectoryInfo(productPath4);
                            if (dir4.Exists) dir4.Delete(true);
                        }
                        if (!string.IsNullOrEmpty(productPath5))
                        {
                            DirectoryInfo dir5 = new DirectoryInfo(productPath5);
                            if (dir5.Exists) dir5.Delete(true);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(T("Error when trign delete directory for products --------------------------------------->" + e.Message).ToString());
                        resultError = true;
                    }
                    finally
                    {
                        var serializer = new JavaScriptSerializer();
                        serializer.MaxJsonLength = int.MaxValue;
                        var data = serializer.Deserialize<DesignInfo>(campaign.Design);
                        var color = _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[0])).First();
                        
                        var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                        var frontPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_front.png");
                        var backPath = Path.Combine(imageFolder, "product_type_" + prod.ProductRecord.Id + "_back.png");

                        try
                        {
                            CreateImagesForOtherColor(campaign.Id, prod.Id.ToString(), prod, data, frontPath, backPath, color.Value);
                            prod.ProductColorRecord = color;

                            if (!string.IsNullOrEmpty(colors[1]))
                            {
                                color = _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[1])).First();
                                CreateImagesForOtherColor(campaign.Id, string.Format("{0}_{1}", prod.Id.ToString(), color.Id.ToString()), prod, data, frontPath, backPath, color.Value);
                                prod.SecondProductColorRecord = color;
                            }
                            else
                            {
                                prod.SecondProductColorRecord = null;
                            }

                            if (!string.IsNullOrEmpty(colors[2]))
                            {
                                color = _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[2])).First();
                                _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[2])).First();
                                CreateImagesForOtherColor(campaign.Id, string.Format("{0}_{1}", prod.Id.ToString(), color.Id.ToString()), prod, data, frontPath, backPath, color.Value);
                                prod.ThirdProductColorRecord = color;
                            }
                            else
                            {
                                prod.ThirdProductColorRecord = null;
                            }

                            if (!string.IsNullOrEmpty(colors[3]))
                            {
                                color = _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[3])).First();
                                _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[3])).First();
                                CreateImagesForOtherColor(campaign.Id, string.Format("{0}_{1}", prod.Id.ToString(), color.Id.ToString()), prod, data, frontPath, backPath, color.Value);
                                prod.FourthProductColorRecord = color;
                            }
                            else
                            {
                                prod.FourthProductColorRecord = null;
                            }

                            if (!string.IsNullOrEmpty(colors[4]))
                            {
                                color = _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[4])).First();
                                _repositoryColor.Table.Where(c => c.Id == Int32.Parse(colors[4])).First();
                                CreateImagesForOtherColor(campaign.Id, string.Format("{0}_{1}", prod.Id.ToString(), color.Id.ToString()), prod, data, frontPath, backPath, color.Value);
                                prod.FifthProductColorRecord = color;
                            }
                            else
                            {
                                prod.FifthProductColorRecord = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(T("Error when trign creating images and directories for products --------------------------------------->" + ex.Message).ToString());
                            resultError = true;
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

            if (resultError)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
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


        public ActionResult DeleteProduct(int productId, int campaignId)
        {
            var camp = _campaignService.GetCampaignById(campaignId);

            try
            {
                camp.Products.Where(c => c.Id == productId).First().WhenDeleted = DateTime.UtcNow;
                _campaignService.UpdateCampaign(camp);
                _notifier.Add(NotifyType.Information, T("The product was removed!"));
            }
            catch
            {
                _notifier.Add(NotifyType.Error, T("An error occurred while deleting"));
            }
            
            return RedirectToAction("ChangeInformation", new { id = campaignId });
        }
    }
}