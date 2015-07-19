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
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class WizardController : Controller
    {
        private readonly ICampaignService _campaignService;

        public WizardController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
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
        public HttpStatusCodeResult LaunchCampaign(LaunchCampaignData data)
        {
            try
            {
                var campaign = _campaignService.CreateNewCampiagn(data);

                for (int i = 0; i < campaign.Products.Count; i++)
                {
                    CreateImagesForCampaignProduct(campaign.Id, campaign.Products[i].Id);
                }
                
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

        private void CreateImagesForCampaignProduct(int campaignId, int productId)
        {
            // TODO: eugene: implement method, now fake
            var imageNum = new Random().Next(1, 4);
            var srcFolder = Path.Combine(Server.MapPath("Media/samples/"), imageNum.ToString());
            var destForder = Path.Combine(Server.MapPath("Media/campaigns/"), campaignId.ToString(), productId.ToString());

            if (!Directory.Exists(destForder))
            {
                Directory.CreateDirectory(destForder + "/normal");                
                Directory.CreateDirectory(destForder + "/big");
            }

            var front = Image.FromFile(Path.Combine(srcFolder, "front.png"));
            var back = Image.FromFile(Path.Combine(srcFolder, "back.png"));

            front.Save(Path.Combine(destForder, "normal", "front.png"), ImageFormat.Png);
            back.Save(Path.Combine(destForder, "normal", "back.png"), ImageFormat.Png);

            var frontLarge = ResizeImage(front, 1070, 1274);
            var backLarge = ResizeImage(back, 1070, 1274);

            frontLarge.Save(Path.Combine(destForder, "big", "front.png"), ImageFormat.Png);
            backLarge.Save(Path.Combine(destForder, "big", "back.png"), ImageFormat.Png);

            frontLarge.Dispose();
            backLarge.Dispose();          
            front.Dispose();
            back.Dispose();
            
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
    }
}