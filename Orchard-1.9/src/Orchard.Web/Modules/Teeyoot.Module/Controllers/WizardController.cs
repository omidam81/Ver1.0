﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Themes;
using RM.QuickLogOn.OAuth.Models;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class WizardController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICampaignService _campaignService;
        private readonly IimageHelper _imageHelper;
        private readonly IFontService _fontService;
        private readonly IProductService _productService;
        private readonly ISwatchService _swatchService;
        private readonly ITShirtCostService _costService;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IRepository<CommonSettingsRecord> _commonSettingsRepository;
        private readonly IRepository<ArtRecord> _artRepository;
        private readonly IRepository<CheckoutCampaignRequest> _checkoutCampaignRequestRepository;
        private readonly ShellSettings _shellSettings;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;
        private readonly IRepository<CurrencyRecord> _currencyRepository;

        private const int ArtsPageSize = 30;
        private const string SendEmailRequestAcceptedKey = "SendEmailAcceptedRequest";
        private const string InvalidEmailKey = "InvalidEmail";

        public WizardController(IOrchardServices orchardServices, ICampaignService campaignService, IimageHelper imageHelper, IFontService fontService, IProductService productService, ISwatchService swatchService, ITShirtCostService costService, ITeeyootMessagingService teeyootMessagingService, IRepository<CommonSettingsRecord> commonSettingsRepository, IRepository<ArtRecord> artRepository, IRepository<CheckoutCampaignRequest> checkoutCampaignRequestRepository, ShellSettings shellSettings, IWorkContextAccessor workContextAccessor, IRepository<CurrencyRecord> currencyRepository)
        {
            _orchardServices = orchardServices;
            _campaignService = campaignService;
            _imageHelper = imageHelper;
            _fontService = fontService;
            _productService = productService;
            _swatchService = swatchService;
            Logger = NullLogger.Instance;
            _costService = costService;
            _teeyootMessagingService = teeyootMessagingService;
            _commonSettingsRepository = commonSettingsRepository;
            _checkoutCampaignRequestRepository = checkoutCampaignRequestRepository;
            _shellSettings = shellSettings;
            T = NullLocalizer.Instance;
            _artRepository = artRepository;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            _currencyRepository = currencyRepository;
        }

        public ILogger Logger { get; set; }

        private Localizer T { get; set; }

        // GET: Wizard
        public ActionResult Index(int? id)
        {
            var commonSettings = _commonSettingsRepository.Table.Where(s=> s.CommonCulture == cultureUsed).FirstOrDefault();
            if (commonSettings == null)
            {
                _commonSettingsRepository.Create(new CommonSettingsRecord() { DoNotAcceptAnyNewCampaigns = false, CommonCulture = cultureUsed });
                commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).First();
            }
            if (commonSettings.DoNotAcceptAnyNewCampaigns)
            {
                return RedirectToAction("Oops");
            }

            var product = _productService.GetAllProducts().Where(pr => pr.ProductHeadlineRecord.ProdHeadCulture == cultureUsed);
            var group = _productService.GetAllProductGroups().Where(gr => gr.ProdGroupCulture == cultureUsed);
            var color = _productService.GetAllColorsAvailable().Where(col => col.ProdColorCulture == cultureUsed);
            //var art = _artRepository.Table.Where(a => a.ArtCulture == cultureUsed);
            var font = _fontService.GetAllfonts().Where(f => f.FontCulture == cultureUsed);
            var swatch = _swatchService.GetAllSwatches().Where(s => s.SwatchCulture == cultureUsed);
            var currencys = _currencyRepository.Table.Where(c => c.CurrencyCulture == cultureUsed);
            var sizes = _productService.GetAllProducts().Where(pr => pr.ProductImageRecord.ProdImgCulture == cultureUsed);
            //var images = _productService.GetAllProducts().Where(pr => pr.);
            if ((product == null || product.Count() == 0) || (group == null || group.Count() == 0) || (color == null || color.Count() == 0) || (font == null || font.Count() == 0) || (swatch == null || swatch.Count() == 0) || (currencys == null || currencys.Count() == 0) || (sizes == null || sizes.Count() == 0))
            {
                return RedirectToAction("Oops");
            }


            var cost = _costService.GetCost(cultureUsed);
            AdminCostViewModel costViewModel = new AdminCostViewModel();
            if (cost != null)
            {
                costViewModel.AdditionalScreenCosts = cost.AdditionalScreenCosts.ToString();
                costViewModel.DTGPrintPrice = cost.DTGPrintPrice.ToString();
                costViewModel.FirstScreenCost = cost.FirstScreenCost.ToString();
                costViewModel.InkCost = cost.InkCost.ToString();
                costViewModel.LabourCost = cost.LabourCost.ToString();
                costViewModel.LabourTimePerColourPerPrint = cost.LabourTimePerColourPerPrint;
                costViewModel.LabourTimePerSidePrintedPerPrint = cost.LabourTimePerSidePrintedPerPrint;
                costViewModel.PercentageMarkUpRequired = cost.PercentageMarkUpRequired.ToString();
                costViewModel.PrintsPerLitre = cost.PrintsPerLitre;
                costViewModel.SalesGoal = cost.SalesGoal;
                costViewModel.MaxColors = cost.MaxColors;
                costViewModel = ReplaceAllCost(costViewModel);
            }

            if (id != null && id > 0)
            {
                int campaignId = (int)id;
                var campaign = _campaignService.GetCampaignById(campaignId);
                var products = _campaignService.GetProductsOfCampaign(campaignId).ToList();
                costViewModel.Campaign = campaign;
                costViewModel.Products = products;
            }

            if (_orchardServices.WorkContext.CurrentUser != null)
            {
                var currentUserRoles = _orchardServices.WorkContext.CurrentUser.ContentItem.As<UserRolesPart>().Roles;
                costViewModel.IsCurrentUserAdministrator = currentUserRoles.Any(r => r == "Administrator");
            }

            var facebookSettingsPart = _orchardServices.WorkContext.CurrentSite.As<FacebookSettingsPart>();
            costViewModel.FacebookClientId = facebookSettingsPart.ClientId;

            var googleSettingsPart = _orchardServices.WorkContext.CurrentSite.As<GoogleSettingsPart>();
            costViewModel.GoogleClientId = googleSettingsPart.ClientId;

            costViewModel.GoogleApiKey = "AIzaSyBijPOV5bUKPNRKTE8areEVNi81ji7sS1I";

            var currency = _currencyRepository.Table.Where(c => c.CurrencyCulture == cultureUsed).First();
            costViewModel.CurrencyCulture = currency.Code;

            return View(costViewModel);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken] Temporary turned off because of not issuing __RequestVerificationToken cookie
        [ValidateInput(false)]
        public HttpStatusCodeResult LaunchCampaign(LaunchCampaignData data)
        {
            if (string.IsNullOrWhiteSpace(data.CampaignTitle) && string.IsNullOrWhiteSpace(data.Description) && string.IsNullOrWhiteSpace(data.Alias))
            {
                string error = "name|" + T("Campiagn Title can't be empty").ToString() + "|campaign_description_text|" + T("Campiagn Description can't be empty").ToString() + "|url|" + T("Campiagn URL can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.CampaignTitle) && string.IsNullOrWhiteSpace(data.Description))
            {
                string error = "name|" + T("Campiagn Title can't be empty").ToString() + "|campaign_description_text|" + T("Campiagn Description can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.CampaignTitle) && string.IsNullOrWhiteSpace(data.Alias))
            {
                string error = "name|" + T("Campiagn Title can't be empty").ToString() + "|url|" + T("Campiagn URL can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.Description) && string.IsNullOrWhiteSpace(data.Alias))
            {
                string error = "campaign_description_text|" + T("Campiagn Description can't be empty").ToString() + "|url|" + T("Campiagn URL can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.CampaignTitle))
            {
                string error = "name|" + T("Campiagn Title can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.Description))
            {
                string error = "campaign_description_text|" + T("Campiagn Description can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.Alias))
            {
                string error = "url|" + T("Campiagn URL can't be empty").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            data.Alias = data.Alias.Trim();

            if (data.Alias.Any(ch => Char.IsWhiteSpace(ch)))
            {
                string error = "url|" + T("Campiagn URL can't contain whitespaces").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (data.Alias.Contains('&') || data.Alias.Contains('?') || data.Alias.Contains('/') || data.Alias.Contains('\\'))
            {
                string error = "url|" + T("Campiagn URL has wrong format").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (_campaignService.GetCampaignByAlias(data.Alias) != null)
            {
                string error = "url|" + T("Campiagn with this URL already exists").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }
            if (data.Alias.Length < 4)
            {
                string error = "url|" + T("Campiagn URL must be at least 4 characters long").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (string.IsNullOrWhiteSpace(data.Design))
            {
                string error = "Design|" + T("No design found for your campaign").ToString();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                foreach (var prod in data.Products)
                {
                    double price = 0;
                    if (!double.TryParse(prod.Price, out price))
                    {
                        double.TryParse(prod.Price.Replace('.', ','), out price);
                    }
                    double cost = 0;
                    if (!double.TryParse(prod.BaseCost, out cost))
                    {
                        double.TryParse(prod.BaseCost.Replace('.', ','), out cost);
                    }

                    if (price < cost)
                    {
                        prod.Price = prod.BaseCost;
                    }
                }

                data.CampaignCulture = cultureUsed; ////TODO: (auth:keinlekan) После удаления поля в таблице/моделе - удалить данный код

                var campaign = _campaignService.CreateNewCampiagn(data);
                CreateImagesForCampaignProducts(campaign);
                var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                _teeyootMessagingService.SendNewCampaignAdminMessage(pathToTemplates, pathToMedia, campaign.Id);
               
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error("Error occured when trying to create new campaign ---------------> " + e.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, T("Error occured when trying to create new campaign").ToString());
            }
        }

        [HttpPost]
        public JsonResult UpoadArtFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileExt = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid() + fileExt;
                var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/uploads/"), fileName);
                file.SaveAs(path);
                return Json(new ArtUpload { Name = fileName });
            }
            return null;
        }

        public JsonResult GetDetailTags(string filter)
        {
            var entries = _campaignService.GetAllCategories().Where(c => c.CategoriesCulture == cultureUsed).Select(n => new { name = n.Name }).ToList();
            return Json(entries, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFonts()
        {
            var fonts = _fontService.GetAllfonts().Where(f => f.FontCulture == cultureUsed);
            return Json(fonts.Select(f => new { id = f.Id, family = f.Family, filename = f.FileName, tags = f.Tags, priority = f.Priority }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSwatches()
        {
            var swatches = _swatchService.GetAllSwatches().Where(s => s.SwatchCulture == cultureUsed);
            return Json(swatches.ToList().Select(s => new { id = s.Id, name = s.Name, inStock = s.InStock, rgb = new[] { s.Red, s.Green, s.Blue } }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArts(string query, int page)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<ArtItemDto>(), JsonRequestBehavior.AllowGet);
            }

            var arts = _artRepository.Table
                .Where(a => a.Name.Contains(query.ToLowerInvariant()) && a.ArtCulture == cultureUsed);

            if (page > 0)
            {
                arts = arts.Skip((page - 1)*ArtsPageSize);
            }

            arts = arts.Take(ArtsPageSize);

            var artDtos = arts.ToList()
                .Select(a => new ArtItemDto
                {
                    id = a.Id,
                    name = a.Name,
                    filename = a.FileName
                });

            return Json(artDtos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRandomArts()
        {
            var arts = new List<ArtItemDto>();

            using (var connection = new SqlConnection(_shellSettings.DataConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = " SELECT TOP (@artsPageSize) * FROM Teeyoot_Module_ArtRecord WHERE ArtCulture = @artCulture ORDER BY NEWID()";

                        var artsPageSizeParameter = new SqlParameter("@artsPageSize", SqlDbType.Int)
                        {
                            Value = ArtsPageSize
                        };
                        var artCulture = new SqlParameter("@artCulture", SqlDbType.VarChar)
                        {
                            Value = cultureUsed
                        };
                        command.Parameters.Add(artsPageSizeParameter);
                        command.Parameters.Add(artCulture);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var artItem = new ArtItemDto
                                {
                                    id = (int) reader["Id"],
                                    name = (string) reader["Name"],
                                    filename = (string) reader["FileName"]
                                };

                                arts.Add(artItem);
                            }
                        }
                    }
                    transaction.Commit();
                }
            }

            return Json(arts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts()
        {
            var model = new WizardProductsViewModel();

            var products = _productService.GetAllProducts().Where(pr => pr.ProductHeadlineRecord.ProdHeadCulture == cultureUsed).ToList();
            var groups = _productService.GetAllProductGroups().Where(gr => gr.ProdGroupCulture == cultureUsed).ToList();
            var colors = _productService.GetAllColorsAvailable().Where(col => col.ProdColorCulture == cultureUsed);

            model.product_colors = colors.Select(c => new ColorViewModel
            {
                id = c.Id,
                name = c.Name,
                value = c.Value,
                importance = c.Importance
            }).ToArray();

            model.product_images = products.Select(p => new ProductImageViewModel
            {
                id = p.ProductImageRecord.Id,
                product_id = p.Id,
                width = p.ProductImageRecord.Width,
                height = p.ProductImageRecord.Height,
                ppi = p.ProductImageRecord.Ppi,
                printable_back_height = p.ProductImageRecord.PrintableBackHeight,
                printable_back_left = p.ProductImageRecord.PrintableBackLeft,
                printable_back_top = p.ProductImageRecord.PrintableBackTop,
                printable_back_width = p.ProductImageRecord.PrintableBackWidth,
                printable_front_height = p.ProductImageRecord.PrintableFrontHeight,
                printable_front_left = p.ProductImageRecord.PrintableFrontLeft,
                printable_front_top = p.ProductImageRecord.PrintableFrontTop,
                printable_front_width = p.ProductImageRecord.PrintableFrontWidth,
                chest_line_back = p.ProductImageRecord.ChestLineBack,
                chest_line_front = p.ProductImageRecord.ChestLineFront,
                gender = p.ProductImageRecord.Gender
            }).ToArray();

            model.product_groups = groups.Select(g => new ProductGroupViewModel
            {
                id = g.Id,
                name = g.Name,
                singular = g.Name.ToLower(),
                products = g.Products.Where(c => c.ProductRecord.WhenDeleted == null).Select(pr => pr.ProductRecord.Id).ToArray()
            }).ToArray();

            model.products = products.Select(p => new ProductViewModel
            {
                id = p.Id,
                name = p.Name,
                headline = p.ProductHeadlineRecord.Name,
                colors_available = p.ColorsAvailable.Select(c => c.ProductColorRecord.Id).ToArray(),
                list_of_sizes = p.SizesAvailable.Count > 0 ?
                    p.SizesAvailable
                    .OrderBy(s => s.ProductSizeRecord.SizeCodeRecord.Id)
                    .First()
                    .ProductSizeRecord
                    .SizeCodeRecord.Name + " - " + 
                    p.SizesAvailable
                    .OrderBy(s => s.ProductSizeRecord.SizeCodeRecord.Id)
                    .Last()
                    .ProductSizeRecord.SizeCodeRecord.Name :
                    "",
                prices = p.ColorsAvailable.Select(c => new ProductPriceViewModel
                {
                    color_id = c.ProductColorRecord.Id,
                    price = p.BaseCost
                }).ToArray()
            }).ToArray();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetProductsAsync()
        {
            var model = new WizardProductsViewModel();

            var colorTask = GetColorViewModelsAsync();
            var productTask = GetProductViewModelsAsync();
            var groupTask = GetProductGroupViewModelsAsync();
            var imageTask = GetProductImageViewModelsAsync();

            await Task.WhenAll(colorTask, productTask, groupTask, imageTask);

            model.product_colors = colorTask.Result;
            model.products = productTask.Result;
            model.product_groups = groupTask.Result;
            model.product_images = imageTask.Result;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Helper methods

        private object _locker = new object();

        private Task<ColorViewModel[]> GetColorViewModelsAsync()
        {
            var tcs = new TaskCompletionSource<ColorViewModel[]>();
            Task.Run(() =>
            {
                try
                {
                    lock (_locker)
                    {
                        var result = _productService.GetAllColorsAvailable().Where(c => c.ProdColorCulture == cultureUsed).Select(c => new ColorViewModel
                            {
                                id = c.Id,
                                name = c.Name,
                                value = c.Value,
                                importance = c.Importance
                            }).ToArray();
                        tcs.SetResult(result);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private Task<ProductViewModel[]> GetProductViewModelsAsync()
        {
            var tcs = new TaskCompletionSource<ProductViewModel[]>();

            Task.Run(() =>
            {
                try
                {
                    lock (_locker)
                    {
                        var result = _productService.GetAllProducts().Where(pr => pr.ProductHeadlineRecord.ProdHeadCulture == cultureUsed).ToList().Select(p => new ProductViewModel
                            {
                                id = p.Id,
                                name = p.Name,
                                headline = p.ProductHeadlineRecord.Name,
                                colors_available = p.ColorsAvailable.Select(c => c.ProductColorRecord.Id).ToArray(),
                                list_of_sizes = p.SizesAvailable.Count > 0 ?
                                    p.SizesAvailable.First().ProductSizeRecord.SizeCodeRecord.Name + " - " + p.SizesAvailable.Last().ProductSizeRecord.SizeCodeRecord.Name :
                                    ""
                            }).ToArray();

                        tcs.SetResult(result);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private Task<ProductGroupViewModel[]> GetProductGroupViewModelsAsync()
        {
            var tcs = new TaskCompletionSource<ProductGroupViewModel[]>();

            Task.Run(() =>
            {
                try
                {
                    lock (_locker)
                    {
                        var groups = _productService.GetAllProductGroups().Where(gr => gr.ProdGroupCulture == cultureUsed).ToList().Select(g => new ProductGroupViewModel
                            {
                                id = g.Id,
                                name = g.Name,
                                singular = g.Name.ToLower(),
                                products = g.Products.Select(pr => pr.ProductRecord.Id).ToArray()
                            }).ToArray();

                        tcs.SetResult(groups);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private Task<ProductImageViewModel[]> GetProductImageViewModelsAsync()
        {
            var tcs = new TaskCompletionSource<ProductImageViewModel[]>();

            Task.Run(() =>
            {
                try
                {
                    lock (_locker)
                    {
                        var images = _productService.GetAllProducts().Where(pr => pr.ProductHeadlineRecord.ProdHeadCulture == cultureUsed).Select(p => new ProductImageViewModel
                            {
                                id = p.ProductImageRecord.Id,
                                product_id = p.Id,
                                width = p.ProductImageRecord.Width,
                                height = p.ProductImageRecord.Height,
                                ppi = p.ProductImageRecord.Ppi,
                                printable_back_height = p.ProductImageRecord.PrintableBackHeight,
                                printable_back_left = p.ProductImageRecord.PrintableBackLeft,
                                printable_back_top = p.ProductImageRecord.PrintableBackTop,
                                printable_back_width = p.ProductImageRecord.PrintableBackWidth,
                                printable_front_height = p.ProductImageRecord.PrintableFrontHeight,
                                printable_front_left = p.ProductImageRecord.PrintableFrontLeft,
                                printable_front_top = p.ProductImageRecord.PrintableFrontTop,
                                printable_front_width = p.ProductImageRecord.PrintableFrontWidth,
                                chest_line_back = p.ProductImageRecord.ChestLineBack,
                                chest_line_front = p.ProductImageRecord.ChestLineFront,
                                gender = p.ProductImageRecord.Gender
                            }).ToArray();

                        tcs.SetResult(images);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private void CreateImagesForCampaignProducts(CampaignRecord campaign)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var data = serializer.Deserialize<DesignInfo>(campaign.Design);

            foreach (var p in campaign.Products)
            {
                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                var frontPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_front.png");
                var backPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_back.png");

                CreateImagesForOtherColor(campaign.Id, p.Id.ToString(), p, data, frontPath, backPath, p.ProductColorRecord.Value);

                if (p.SecondProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.SecondProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.SecondProductColorRecord.Value);
                }
                if (p.ThirdProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.ThirdProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.ThirdProductColorRecord.Value);
                }
                if (p.FourthProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.FourthProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.FourthProductColorRecord.Value);
                }
                if (p.FifthProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.FifthProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.FifthProductColorRecord.Value);
                }

            }
                //var destForder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), p.Id.ToString());

                //if (!Directory.Exists(destForder))
                //{
                //    Directory.CreateDirectory(destForder + "/normal");
                //    Directory.CreateDirectory(destForder + "/big");
                //}

                //var frontTemplate = new Bitmap(frontPath);
                //var backTemplate = new Bitmap(backPath);

                //var rgba = ColorTranslator.FromHtml(p.ProductColorRecord.Value);

                //var front = BuildProductImage(frontTemplate, _imageHelper.Base64ToBitmap(data.Front), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                //    p.ProductRecord.ProductImageRecord.PrintableFrontTop, p.ProductRecord.ProductImageRecord.PrintableFrontLeft,
                //    p.ProductRecord.ProductImageRecord.PrintableFrontWidth, p.ProductRecord.ProductImageRecord.PrintableFrontHeight);
                //front.Save(Path.Combine(destForder, "normal", "front.png"));

                //var back = BuildProductImage(backTemplate, _imageHelper.Base64ToBitmap(data.Back), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                //    p.ProductRecord.ProductImageRecord.PrintableBackTop, p.ProductRecord.ProductImageRecord.PrintableBackLeft,
                //    p.ProductRecord.ProductImageRecord.PrintableBackWidth, p.ProductRecord.ProductImageRecord.PrintableBackHeight);
                //back.Save(Path.Combine(destForder, "normal", "back.png"));

                //_imageHelper.ResizeImage(front, 1070, 1274).Save(Path.Combine(destForder, "big", "front.png"));
                //_imageHelper.ResizeImage(back, 1070, 1274).Save(Path.Combine(destForder, "big", "back.png"));

                //frontTemplate.Dispose();
                //backTemplate.Dispose();
                //front.Dispose();
                //back.Dispose();

                int product = _campaignService.GetProductsOfCampaign(campaign.Id).First().Id;
                string destFolder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), product.ToString(), "social");
                Directory.CreateDirectory(destFolder);

                var imageSocialFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                if (!campaign.BackSideByDefault)
                {
                    var frontSocialPath = Path.Combine(imageSocialFolder, "product_type_" + campaign.Products.Where(pr=>pr.WhenDeleted ==null).First().ProductRecord.Id + "_front.png");
                    var imgPath = new Bitmap(frontSocialPath);

                    _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Front);
                }
                else
                {
                    var backSocialPath = Path.Combine(imageSocialFolder, "product_type_" + campaign.Products.Where(pr=>pr.WhenDeleted == null).First().ProductRecord.Id + "_back.png");
                    var imgPath = new Bitmap(backSocialPath);

                    _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Back);
                }           
        }

        private Bitmap BuildProductImage(Bitmap image, Bitmap design, Color color, int width, int height, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight)
        {
            var background = _imageHelper.CreateBackground(width, height, color);
            image = _imageHelper.ApplyBackground(image, background, width, height);
            return _imageHelper.ApplyDesign(image, design, printableAreaTop, printableAreaLeft, printableAreaWidth, printableAreaHeight, width, height);
        }

        #endregion

        public AdminCostViewModel ReplaceAllCost(AdminCostViewModel cost)
        {
            cost.AdditionalScreenCosts = cost.AdditionalScreenCosts.Replace(",", ".");
            cost.DTGPrintPrice = cost.DTGPrintPrice.Replace(",", ".");
            cost.FirstScreenCost = cost.FirstScreenCost.Replace(",", ".");
            cost.InkCost = cost.InkCost.Replace(",", ".");
            cost.LabourCost = cost.LabourCost.Replace(",", ".");
            cost.PercentageMarkUpRequired = cost.PercentageMarkUpRequired.Replace(",", ".");

            return cost;
        }

        public ActionResult Oops()
        {
            var viewModel = new OopsViewModel();

            if (TempData[InvalidEmailKey] != null)
            {
                viewModel.InvalidEmail = (bool) TempData[InvalidEmailKey];
            }

            if (TempData[SendEmailRequestAcceptedKey] != null)
            {
                viewModel.RequestAccepted = (bool) TempData[SendEmailRequestAcceptedKey];
            }
            
            return View(viewModel);
        }
        
        [HttpPost]
        public ActionResult Oops(OopsViewModel viewModel)
        {
            var commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).FirstOrDefault();
            if (commonSettings == null)
            {
                _commonSettingsRepository.Create(new CommonSettingsRecord() { DoNotAcceptAnyNewCampaigns = false, CommonCulture = cultureUsed });
                commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).First();
            }
            if (!commonSettings.DoNotAcceptAnyNewCampaigns)
            {
                return RedirectToAction("Oops");
            }

            if (!ModelState.IsValidField("Email"))
            {
                TempData[InvalidEmailKey] = true;
                return RedirectToAction("Oops");
            }

            var request = new CheckoutCampaignRequest {RequestUtcDate = DateTime.UtcNow, Email = viewModel.Email};
            _checkoutCampaignRequestRepository.Create(request);

            TempData[SendEmailRequestAcceptedKey] = true;
            return RedirectToAction("Oops");
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


            var frontZoom = BuildProductImage(frontTemplate, _imageHelper.Base64ToBitmap(data.Front), rgba, p.ProductRecord.ProductImageRecord.Width * 4, p.ProductRecord.ProductImageRecord.Height * 4,
                p.ProductRecord.ProductImageRecord.PrintableFrontTop * 4, p.ProductRecord.ProductImageRecord.PrintableFrontLeft *4,
                p.ProductRecord.ProductImageRecord.PrintableFrontWidth * 4, p.ProductRecord.ProductImageRecord.PrintableFrontHeight * 4);
          
            var backZoom = BuildProductImage(backTemplate, _imageHelper.Base64ToBitmap(data.Back), rgba, p.ProductRecord.ProductImageRecord.Width * 4, p.ProductRecord.ProductImageRecord.Height * 4,
                p.ProductRecord.ProductImageRecord.PrintableBackTop * 4, p.ProductRecord.ProductImageRecord.PrintableBackLeft * 4,
                p.ProductRecord.ProductImageRecord.PrintableBackWidth * 4, p.ProductRecord.ProductImageRecord.PrintableBackHeight * 4);

            Rectangle rect = new Rectangle(0, 0, frontZoom.Width - 10, frontZoom.Height - 10);
            Bitmap croppedFront = frontZoom.Clone(rect, frontZoom.PixelFormat);

            croppedFront.Save(Path.Combine(destForder, "big", "front.png"));

            Rectangle rect2 = new Rectangle(0, 0, backZoom.Width - 10, backZoom.Height - 10);
            Bitmap croppedBck = backZoom.Clone(rect2, backZoom.PixelFormat);

            croppedBck.Save(Path.Combine(destForder, "big", "back.png"));

            frontTemplate.Dispose();
            backTemplate.Dispose();
            croppedFront.Dispose();
            croppedBck.Dispose();
            front.Dispose();
            back.Dispose();
        }
    }
}