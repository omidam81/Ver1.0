using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ArtworkController : Controller
    {
        private readonly IOrchardServices _orchardServices;

        private const string ArtworksImagesRelativePath = "~/Modules/Teeyoot.Module/Content/vector";
        private const string ArtworkImageFileNameTemplate = "{0}.png";

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ArtworkController(
            IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddArtwork()
        {
            var viewModel = new ArtworkViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddArtwork(ArtworkViewModel viewModel)
        {
            var success = SaveArtworkImage(viewModel.ArtworkImage, viewModel.Name);
            if (success)
            {
                _orchardServices.Notifier.Information(T("Artwork has been successfully added."));
            }

            return RedirectToAction("Index");
        }

        private bool SaveArtworkImage(HttpPostedFileBase imageFile, string fileName)
        {
            if (imageFile == null)
            {
                _orchardServices.Notifier.Error(T("Image file was not provided"));
                return false;
            }

            if (CheckIfAlreadyExists(fileName) != null)
            {
                _orchardServices.Notifier.Error(T("Image with the same name already exist"));
                return false;
            }

            if (!IsImagePng(imageFile))
            {
                _orchardServices.Notifier.Error(T("Image file should be *.png"));
                return false;
            }

            var imageFileName = string.Format(ArtworkImageFileNameTemplate, fileName);
            var imagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), imageFileName);

            imageFile.SaveAs(imagePhysicalPath);

            return true;
        }

        private static bool IsImagePng(HttpPostedFileBase imageFile)
        {
            var imageHeader = new byte[4];

            imageFile.InputStream.Read(imageHeader, 0, 4);
            var strHeader = Encoding.ASCII.GetString(imageHeader);

            return strHeader.ToLowerInvariant().EndsWith("png");
        }

        private string CheckIfAlreadyExists(string fileName)
        {
            var imageFileName = string.Format(ArtworkImageFileNameTemplate, fileName);
            var imagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), imageFileName);

            return System.IO.File.Exists(imagePhysicalPath) ? imageFileName : null;
        }
    }
}