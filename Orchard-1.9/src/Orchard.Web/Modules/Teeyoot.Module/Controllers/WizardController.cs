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
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class WizardController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly IimageHelper _imageHelper;
        private readonly IFontService _fontService;
        private readonly IProductService _productService;
        private readonly ISwatchService _swatchService;

        public WizardController(ICampaignService campaignService, IimageHelper imageHelper, IFontService fontService, IProductService productService, ISwatchService swatchService)
        {
            _campaignService = campaignService;
            _imageHelper = imageHelper;
            _fontService = fontService;
            _productService = productService;
            _swatchService = swatchService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        // GET: Wizard
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public HttpStatusCodeResult LaunchCampaign(LaunchCampaignData data)
        {
            if (string.IsNullOrWhiteSpace(data.CampaignTitle))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Campiagn Title can't be empty");
            }

            if (string.IsNullOrWhiteSpace(data.Description))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Campiagn Description can't be empty");
            }

            if (string.IsNullOrWhiteSpace(data.Alias))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Campiagn URL can't be empty");
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
            var entries = _campaignService.GetAllCategories().Where(c => c.Name.Contains(filter)).Select(n => n.Name).Take(10).ToList();
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
                    ""
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

        private Task<ColorViewModel[]> GetColorViewModelsAsync()
        {
            var tcs = new TaskCompletionSource<ColorViewModel[]>();
            Task.Run(() => {
                try
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
                catch(Exception ex)
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
                    var groups = _productService.GetAllProductGroups().ToList().Select(g => new ProductGroupViewModel
                        {
                            id = g.Id,
                            name = g.Name,
                            singular = g.Name.ToLower(),
                            products = g.Products.Select(pr => pr.ProductRecord.Id).ToArray()
                        }).ToArray();

                    tcs.SetResult(groups);
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
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        private void CreateImagesForCampaignProducts(CampaignRecord campaign)
        {
            var data = new JavaScriptSerializer().Deserialize<DesignInfo>(campaign.Design);

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

                ResizeImage(front, 1070, 1274).Save(Path.Combine(destForder, "big", "front.png"));
                ResizeImage(back, 1070, 1274).Save(Path.Combine(destForder, "big", "back.png"));

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

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        #endregion

    }
}