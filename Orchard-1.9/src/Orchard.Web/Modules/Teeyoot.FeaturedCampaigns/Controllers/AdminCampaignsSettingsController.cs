using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DataTables.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Teeyoot.FeaturedCampaigns.Common;
using Teeyoot.FeaturedCampaigns.Models;
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
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly IContentManager _contentManager;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IOrderService _orderService;
        private readonly INotifier _notifier;
        private readonly IRepository<ProductColorRecord> _repositoryColor;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ShellSettings _shellSettings;
        private readonly string _cultureUsed;

        public AdminCampaignsSettingsController(
            ICampaignService campaignService,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IimageHelper imageHelper,
            IOrderService orderService,
            IContentManager contentManager,
            ITeeyootMessagingService teeyootMessagingService,
            INotifier notifier,
            IRepository<ProductColorRecord> repositoryColor,
            IWorkContextAccessor workContextAccessor,
            ShellSettings shellSettings,
            IRepository<CurrencyRecord> currencyRepository)
        {
            _campaignService = campaignService;
            _siteService = siteService;
            _imageHelper = imageHelper;
            _contentManager = contentManager;
            _teeyootMessagingService = teeyootMessagingService;
            _orderService = orderService;
            _currencyRepository = currencyRepository;

            Shape = shapeFactory;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _notifier = notifier;
            _repositoryColor = repositoryColor;

            _shellSettings = shellSettings;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            _cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var total = _campaignService.GetAllCampaigns().Where(c => c.CampaignCulture == _cultureUsed).Count();
            var orderedProducts = _orderService.GetAllOrderedProducts();

            var campaigns = _campaignService.GetAllCampaigns().Where(c => c.CampaignCulture == _cultureUsed).Select(c => new
            {
                Id = c.Id,
                Title = c.Title,
                Goal = c.ProductCountGoal,
                Sold = c.ProductCountSold,
                Status = c.CampaignStatusRecord.Name,
                UserId = c.TeeyootUserId,
                IsApproved = c.IsApproved,
                EndDate = c.EndDate,
                Profit = orderedProducts
                                    .Where(p => p.OrderRecord.IsActive && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.OrderStatusRecord.Name != "Unapproved" && p.CampaignProductRecord.CampaignRecord_Id == c.Id)
                                    .Select(pr => new { Profit = pr.Count * (pr.CampaignProductRecord.Price - pr.CampaignProductRecord.BaseCost) })
                                    .Sum(entry => (double?)entry.Profit) ?? 0,              
                Alias = c.Alias,
                IsActive = c.IsActive,
                Minimum = c.ProductMinimumGoal,
                CreateDate = c.StartDate
            })
                                .ToList()
                                .OrderBy(e => e.Title);
            var totalNotApproved = _campaignService.GetAllCampaigns().Where(c => c.IsApproved == false && c.Rejected == false && c.CampaignCulture == _cultureUsed).Count();

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday && o.CurrencyRecord.CurrencyCulture == _cultureUsed && o.OrderStatusRecord.Name != OrderStatus.Cancelled.ToString() && o.OrderStatusRecord.Name != OrderStatus.Unapproved.ToString());

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
                    SummaryCurrency : _currencyRepository.Table.Where(cm => cm.CurrencyCulture == (_campaignService.GetCampaignById(e.Id).CampaignCulture)).FirstOrDefault().Code,
                    Alias: e.Alias,
                    IsActive: e.IsActive,
                    Minimum: e.Minimum,
                    CreatedDate: e.CreateDate.ToLocalTime().ToString("dd/MM/yyyy"),
                    Last24HoursSold: last24hoursOrders
                                        .SelectMany(o => o.Products)
                                        .Where(p => p.CampaignProductRecord.CampaignRecord_Id == e.Id )
                                        .Sum(p => (int?)p.Count) ?? 0
                    );
            });

            return View(new ExportPrintsViewModel { Campaigns = entriesProjection.ToArray(), NotApprovedTotal = totalNotApproved });
        }

        public JsonResult GetCampaigns(
            [ModelBinder(typeof (GetCampaignsDataTablesBinder))] GetCampaignsDataTablesRequest request)
        {
            IEnumerable<CampaignItemViewModel> campaignItemViewModels;

            using (var connection = new SqlConnection(_shellSettings.DataConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "GetCampaigns";

                        var currentDateParameter = new SqlParameter("@CurrentDate", SqlDbType.DateTime)
                        {
                            Value = DateTime.UtcNow
                        };
                        command.Parameters.Add(currentDateParameter);

                        var cultureParameter = new SqlParameter("@Culture", SqlDbType.NVarChar, 50)
                        {
                            Value = _cultureUsed
                        };
                        command.Parameters.Add(cultureParameter);

                        if (request.Columns.GetSortedColumns().Any())
                        {
                            var sortColumn = request.Columns.GetSortedColumns().First();
                            var sortColumnParameter = new SqlParameter("@SortColumn", SqlDbType.NVarChar, 100)
                            {
                                Value = sortColumn.Data
                            };
                            command.Parameters.Add(sortColumnParameter);

                            var sortDirection = sortColumn.SortDirection == Column.OrderDirection.Ascendant
                                ? "ASC"
                                : "DESC";
                            var sortDirectionParameter = new SqlParameter("@SortDirection", SqlDbType.NVarChar, 50)
                            {
                                Value = sortDirection
                            };
                            command.Parameters.Add(sortDirectionParameter);
                        }

                        var skipParameter = new SqlParameter("@Skip", SqlDbType.Int)
                        {
                            Value = request.Start
                        };
                        command.Parameters.Add(skipParameter);
                        var takeParameter = new SqlParameter("@Take", SqlDbType.Int)
                        {
                            Value = request.Length
                        };
                        command.Parameters.Add(takeParameter);

                        IEnumerable<CampaignItem> campaignItems;

                        using (var reader = command.ExecuteReader())
                        {
                            campaignItems = GetCampaignItemsFrom(reader);
                        }

                        campaignItemViewModels = ConvertToCampaignItemViewModels(campaignItems);

                        /*

                        var sortColumnParameter = new SqlParameter("@Culture", SqlDbType.NVarChar, 50)
                        {
                            Value = _cultureUsed
                        };
                        */
                        /*
                        var filterParameter = new SqlParameter("@Filter", SqlDbType.NVarChar, 4000)
                        {
                            Value = "%" + request.Filter + "%"
                        };
                        var skipParameter = new SqlParameter("@Skip", SqlDbType.Int)
                        {
                            Value = request.Skip
                        };
                        var takeParameter = new SqlParameter("@Take", SqlDbType.Int)
                        {
                            Value = request.Take
                        };
                         */
                        /*
                        command.Parameters.Add(currentDateParameter);
                        command.Parameters.Add(cultureParameter);
                         */
                        /*
                        command.Parameters.Add(filterParameter);
                        command.Parameters.Add(skipParameter);
                        command.Parameters.Add(takeParameter);
                         */
                        /*
                        using (var reader = command.ExecuteReader())
                        {
                            response.Campaigns = GetSearchCampaignItemsFrom(reader);
                        }
                         */
                    }

                    transaction.Commit();
                }
            }

            return Json(new DataTablesResponse(request.Draw, campaignItemViewModels, 100, 100));
        }

        private static IEnumerable<CampaignItem> GetCampaignItemsFrom(IDataReader reader)
        {
            var campaignItems = new List<CampaignItem>();

            while (reader.Read())
            {
                var campaignItem = new CampaignItem
                {
                    Profit = (double) reader["Profit"],
                    Last24HoursSold = (int) reader["Last24HoursSold"],
                    Id = (int) reader["Id"],
                    Title = (string) reader["Title"],
                    Goal = (int) reader["Goal"],
                    Sold = (int) reader["Sold"],
                    IsApproved = (bool) reader["IsApproved"],
                    EndDate = (DateTime) reader["EndDate"],
                    Alias = (string) reader["Alias"],
                    IsActive = (bool) reader["IsActive"],
                    Minimum = (int) reader["Minimum"],
                    CreatedDate = (DateTime) reader["CreatedDate"],
                    Status = (string) reader["Status"],
                    Email = (string) reader["Email"]
                };

                if (reader["PhoneNumber"] != DBNull.Value)
                    campaignItem.PhoneNumber = (string) reader["PhoneNumber"];

                campaignItems.Add(campaignItem);
            }

            return campaignItems;
        }

        private IEnumerable<CampaignItemViewModel> ConvertToCampaignItemViewModels(
            IEnumerable<CampaignItem> campaignItems)
        {
            return campaignItems.Select(campaignItem => new CampaignItemViewModel
            {
                Id = campaignItem.Id,
                Title = campaignItem.Title,
                Alias = "/" + campaignItem.Alias,
                CreatedDate = campaignItem.CreatedDate.ToLocalTime().ToString("dd/MM/yyyy"),
                IsActive = campaignItem.IsActive ? T("Yes").ToString() : T("No").ToString(),
                Last24HoursSold = campaignItem.Last24HoursSold > 0 ? campaignItem.Last24HoursSold.ToString() : "-",
                Sold = campaignItem.Sold,
                Minimum = campaignItem.Minimum,
                Goal = campaignItem.Goal,
                Profit = campaignItem.Profit.ToString("F", System.Globalization.CultureInfo.InvariantCulture),
                Status = campaignItem.Status,
                EndDate = campaignItem.EndDate.ToLocalTime().ToString("dd/MM/yyyy"),
                Email = campaignItem.Email,
                PhoneNumber = campaignItem.PhoneNumber
            }).ToList();
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
        public HttpStatusCodeResult SaveInfo(int campaignId, string Title, string URL, int Day, int Mounth, int Year, int Target, string Description, string[] Prices, string[] Colors)
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
                    campaign.IsFeatured = false;
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
                
                var prods = campaign.Products.Where(c => c.WhenDeleted == null).ToList();
                for (int i = 0; i < prods.Count; i++)
                {
                    var price = Prices[i].Replace(".", ",");
                    prods[i].Price = Convert.ToDouble(price);
                }
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
                var s = new JavaScriptSerializer();
                s.MaxJsonLength = int.MaxValue;
                var d = s.Deserialize<DesignInfo>(campaign.Design);

                string destFolder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), campaign.Products.Where(pr => pr.WhenDeleted == null).First().Id.ToString(), "social");
                var dirict = new DirectoryInfo(destFolder);
                var imageSocialFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
              
                    Directory.CreateDirectory(destFolder);
            
                    if (campaign.BackSideByDefault == false)
                    {
                        var frontSocialPath = Path.Combine(imageSocialFolder, "product_type_" + campaign.Products.Where(pr => pr.WhenDeleted == null).First().ProductRecord.Id + "_front.png");
                        var imgPath = new Bitmap(frontSocialPath);

                        _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, d.Front);
                    }
                    else
                    {
                        var backSocialPath = Path.Combine(imageSocialFolder, "product_type_" + campaign.Products.Where(pr => pr.WhenDeleted == null).First().ProductRecord.Id + "_back.png");
                        var imgPath = new Bitmap(backSocialPath);

                        _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, d.Back);
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