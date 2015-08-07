using Orchard;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Orchard.ContentManagement;
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

        public WizardController(IOrchardServices orchardServices, ICampaignService campaignService, IimageHelper imageHelper, IFontService fontService, IProductService productService, ISwatchService swatchService, ITShirtCostService costService)
        {
            _orchardServices = orchardServices;
            _campaignService = campaignService;
            _imageHelper = imageHelper;
            _fontService = fontService;
            _productService = productService;
            _swatchService = swatchService;
            Logger = NullLogger.Instance;
            _costService = costService;
        }

        public ILogger Logger { get; set; }

        // GET: Wizard
        public ActionResult Index(int? id)
        {
            var cost = _costService.GetCost();
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

            var facebookSettingsPart = _orchardServices.WorkContext.CurrentSite.As<FacebookSettingsPart>();
            costViewModel.FacebookClientId = facebookSettingsPart.ClientId;

            var googleSettingsPart = _orchardServices.WorkContext.CurrentSite.As<GoogleSettingsPart>();
            costViewModel.GoogleClientId = googleSettingsPart.ClientId;

            costViewModel.GoogleApiKey = "AIzaSyCyvcCc6PJL62UNIoVbgsQcznyjpVhOzuo";

            return View(costViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public HttpStatusCodeResult LaunchCampaign(LaunchCampaignData data)
        {
            //if (string.IsNullOrWhiteSpace(data.CampaignTitle) && string.IsNullOrWhiteSpace(data.Description) && )
            string error = string.Empty;
            bool hasError = false;
            if (string.IsNullOrWhiteSpace(data.CampaignTitle))
            {
                error = "name|Campiagn Title can't be empty";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "name|Campiagn Title can't be empty");
            }

            if (string.IsNullOrWhiteSpace(data.Description))
            {
                error = hasError ? error + "|" + "campaign_description_text|Campiagn Description can't be empty" : "campaign_description_text|Campiagn Description can't be empty";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "campaign_description_text|Campiagn Description can't be empty");
            }

            if (string.IsNullOrWhiteSpace(data.Alias))
            {
                error = hasError ? error + "|" + "url|Campiagn URL can't be empty" : "url|Campiagn URL can't be empty";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "url|Campiagn URL can't be empty");
            }

            data.Alias = data.Alias.Trim();

            if (data.Alias.Any(ch => Char.IsWhiteSpace(ch)))
            {
                error = hasError ? error + "|" + "url|Campiagn URL can't contain whitespaces" : "url|Campiagn URL can't contain whitespaces";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "url|Campiagn URL can't contain whitespaces");
            }

            if (data.Alias.Contains('&') || data.Alias.Contains('?') || data.Alias.Contains('/') || data.Alias.Contains('\\'))
            {
                error = hasError ? error + "|" + "url|Campiagn URL has wrong format" : "url|Campiagn URL has wrong format";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "url|Campiagn URL has wrong format");
            }

            if (_campaignService.GetCampaignByAlias(data.Alias) != null)
            {
                error = hasError ? error + "|" + "url|Campiagn with this URL already exists" : "url|Campiagn with this URL already exists";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "url|Campiagn with this URL already exists");
            }

            if (string.IsNullOrWhiteSpace(data.Design))
            {
                error = hasError ? error + "|" + "url|No design found for your campaign" : "url|No design found for your campaign";
                hasError = true;
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Design|No design found for your campaign");
            }

            if (hasError)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
            }

            if (_orchardServices.WorkContext.CurrentUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            try
            {
                var campaign = _campaignService.CreateNewCampiagn(data);
                CreateImagesForCampaignProducts(campaign);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error("Error occured when trying to create new campaign ---------------> " + e.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occured when trying to create new campaign");
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
            int filterNull = filter.LastIndexOf(' ');
            if (filterNull == filter.Length - 1)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string[] filters = filter.Split(' ');
            string tag = filters[filters.Length - 1];

            var entries = _campaignService.GetAllCategories().Where(c => c.Name.Contains(tag)).Select(n => n.Name).Take(10).ToList();
            return Json(entries, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFonts()
        {
            var fonts = _fontService.GetAllfonts();
            return Json(fonts.Select(f => new { id = f.Id, family = f.Family, filename = f.FileName, tags = f.Tags, priority = f.Priority }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSwatches()
        {
            var swatches = _swatchService.GetAllSwatches();
            return Json(swatches.ToList().Select(s => new { id = s.Id, name = s.Name, inStock = s.InStock, rgb = new[] { s.Red, s.Green, s.Blue } }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts()
        {
            var model = new WizardProductsViewModel();

            var products = _productService.GetAllProducts().ToList();
            var groups = _productService.GetAllProductGroups().ToList();
            var colors = _productService.GetAllColorsAvailable();

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
                products = g.Products.Select(pr => pr.ProductRecord.Id).ToArray()
            }).ToArray();

            model.products = products.Select(p => new ProductViewModel
            {
                id = p.Id,
                name = p.Name,
                headline = p.ProductHeadlineRecord.Name,
                colors_available = p.ColorsAvailable.Select(c => c.ProductColorRecord.Id).ToArray(),
                list_of_sizes = p.SizesAvailable.Count > 0 ?
                    p.SizesAvailable.First().ProductSizeRecord.SizeCodeRecord.Name + " - " + p.SizesAvailable.Last().ProductSizeRecord.SizeCodeRecord.Name :
                    "",
                prices = p.ColorsAvailable.Select(c => new ProductPriceViewModel
                {
                    color_id = c.ProductColorRecord.Id,
                    price = c.BaseCost
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
                        var result = _productService.GetAllColorsAvailable().Select(c => new ColorViewModel
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
                        var result = _productService.GetAllProducts().ToList().Select(p => new ProductViewModel
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
                        var groups = _productService.GetAllProductGroups().ToList().Select(g => new ProductGroupViewModel
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
                        var images = _productService.GetAllProducts().Select(p => new ProductImageViewModel
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
                var destForder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), p.Id.ToString());

                if (!Directory.Exists(destForder))
                {
                    Directory.CreateDirectory(destForder + "/normal");
                    Directory.CreateDirectory(destForder + "/big");
                }

                var frontTemplate = new Bitmap(frontPath);
                var backTemplate = new Bitmap(backPath);

                var rgba = ColorTranslator.FromHtml(p.ProductColorRecord.Value);

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
    }
}