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

        public WizardController(ICampaignService campaignService, IimageHelper imageHelper, IFontService fontService)
        {
            _campaignService = campaignService;
            _imageHelper = imageHelper;
            _fontService = fontService;
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
            return Json(fonts.Select(f => new { id = f.Id, family = f.Family, fileName = f.FileName, tags = f.Tags, priority = f.Priority }), JsonRequestBehavior.AllowGet);
        }

        #region Helper methods

        private void CreateImagesForCampaignProducts(CampaignRecord campaign)
        {
            var data = new JavaScriptSerializer().Deserialize<DesignInfo>(campaign.Design);

            var str = @"data:image/png;base64,";
            if (data.Front.StartsWith(str))
            {
                data.Front = data.Front.Replace(str, "").Trim();
            }
            if (data.Back.StartsWith(str))
            {
                data.Back = data.Back.Replace(str, "").Trim();
            }

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

                var front = BuildProductImage(frontTemplate, _imageHelper.Base64ToBitmap(data.Front), rgba, 
                    p.ProductRecord.ProductImageRecord.PrintableFrontTop, p.ProductRecord.ProductImageRecord.PrintableFrontLeft,
                    p.ProductRecord.ProductImageRecord.PrintableFrontWidth, p.ProductRecord.ProductImageRecord.PrintableFrontHeight);
                front.Save(Path.Combine(destForder, "normal", "front.png"));

                var back = BuildProductImage(backTemplate, _imageHelper.Base64ToBitmap(data.Back), rgba, 
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

        private Bitmap BuildProductImage(Bitmap image, Bitmap design, Color color, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight)
        {
            var background = _imageHelper.CreateBackground(image.Width, image.Height, color);
            image = _imageHelper.ApplyBackground(image, background);
            return _imageHelper.ApplyDesign(image, design, printableAreaTop, printableAreaLeft, printableAreaWidth, printableAreaHeight);
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