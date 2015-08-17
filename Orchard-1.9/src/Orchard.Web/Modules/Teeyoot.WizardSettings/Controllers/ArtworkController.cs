using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ArtworkController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;

        private const string ArtworksImagesRelativePath = "~/Modules/Teeyoot.Module/Content/vector";
        private const string ArtworkSvgImageFileNameTemplate = "{0}.svg";
        private const string ArtworkPngImageFileNameTemplate = "{0}.png";

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ArtworkController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var viewModel = new ArtworkIndexViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var artworkImageFiles = Directory.GetFiles(Server.MapPath(ArtworksImagesRelativePath), "*.png");

            var artworks = artworkImageFiles.Select(it => new ArtworkItemViewModel
            {
                Name = Path.GetFileNameWithoutExtension(it),
                ImageUrl =
                    Url.MakeAbsolute(ArtworksImagesRelativePath + "/" + Path.GetFileName(it),
                        HttpContext.Request.ToRootUrlString())
            })
                .OrderBy(it => it.Name);

            viewModel.Artworks = artworks
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(artworkImageFiles.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }

        public ActionResult AddArtwork()
        {
            var viewModel = new ArtworkViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddArtwork(ArtworkViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddArtwork");
            }

            var success = SaveArtworkImages(viewModel.ArtworkSvgImage, viewModel.ArtworkPngImage, viewModel.Name);
            if (success)
            {
                _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been added", viewModel.Name));
            }

            return RedirectToAction("Index");
        }

        public ActionResult EditArtwork(string name)
        {
            var artworkName =
                Path.GetFileNameWithoutExtension(CheckIfAlreadyExists(name, ArtworkSvgImageFileNameTemplate));

            var viewModel = new ArtworkViewModel
            {
                CurrentName = artworkName,
                Name = artworkName
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditArtwork(ArtworkViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditArtwork");
            }

            var success = UpdateArtworkImages(viewModel.ArtworkSvgImage,
                viewModel.ArtworkPngImage,
                viewModel.Name,
                viewModel.CurrentName);

            return !success
                ? RedirectToAction("EditArtwork", new {name = viewModel.CurrentName})
                : RedirectToAction("Index");
        }

        public ActionResult DeleteArtwork(string name)
        {
            try
            {
                var svgImageFileName = string.Format(ArtworkSvgImageFileNameTemplate, name);
                var svgImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), svgImageFileName);

                System.IO.File.Delete(svgImagePhysicalPath);

                var pngImageFileName = string.Format(ArtworkPngImageFileNameTemplate, name);
                var pngImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), pngImageFileName);

                System.IO.File.Delete(pngImagePhysicalPath);
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting artwork \"{0}\" failed: {1}", name, exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting artwork \"{0}\" failed.", name));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been deleted.", name));
            return RedirectToAction("Index");
        }

        private bool SaveArtworkImages(HttpPostedFileBase svgImageFile, HttpPostedFileBase pngImageFile, string fileName)
        {
            if (svgImageFile == null)
            {
                _orchardServices.Notifier.Error(T("SVG image file was not provided"));
                return false;
            }

            if (pngImageFile == null)
            {
                _orchardServices.Notifier.Error(T("PNG image file was not provided"));
                return false;
            }

            if (CheckIfAlreadyExists(fileName, ArtworkSvgImageFileNameTemplate) != null)
            {
                _orchardServices.Notifier.Error(T("Images with the same name already exist"));
                return false;
            }

            if (!IsImageSvg(svgImageFile))
            {
                _orchardServices.Notifier.Error(T("SVG image file should be *.svg"));
                return false;
            }

            if (!IsImagePng(pngImageFile))
            {
                _orchardServices.Notifier.Error(T("PNG image file should be *.png"));
                return false;
            }

            var svgImageFileName = string.Format(ArtworkSvgImageFileNameTemplate, fileName);
            var svgImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), svgImageFileName);

            svgImageFile.SaveAs(svgImagePhysicalPath);

            var pngImageFileName = string.Format(ArtworkPngImageFileNameTemplate, fileName);
            var pngImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), pngImageFileName);

            pngImageFile.SaveAs(pngImagePhysicalPath);

            return true;
        }

        private bool UpdateArtworkImages(
            HttpPostedFileBase svgImageFile,
            HttpPostedFileBase pngImageFile,
            string newFileName,
            string currentFileName)
        {
            var currentSvgImagePhysicalPath = CheckIfAlreadyExists(currentFileName, ArtworkSvgImageFileNameTemplate);
            var newSvgImageFileName = string.Format(ArtworkSvgImageFileNameTemplate, newFileName);
            var newSvgImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), newSvgImageFileName);

            if (svgImageFile != null)
            {
                System.IO.File.Delete(currentSvgImagePhysicalPath);
                svgImageFile.SaveAs(newSvgImagePhysicalPath);
            }
            else
            {
                System.IO.File.Move(currentSvgImagePhysicalPath, newSvgImagePhysicalPath);
            }

            var currentPngImagePhysicalPath = CheckIfAlreadyExists(currentFileName, ArtworkPngImageFileNameTemplate);
            var newPngImageFileName = string.Format(ArtworkPngImageFileNameTemplate, newFileName);
            var newPngImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), newPngImageFileName);

            if (pngImageFile != null)
            {
                System.IO.File.Delete(currentPngImagePhysicalPath);
                pngImageFile.SaveAs(newPngImagePhysicalPath);
            }
            else
            {
                System.IO.File.Move(currentPngImagePhysicalPath, newPngImagePhysicalPath);
            }

            return true;
        }

        private static bool IsImageSvg(HttpPostedFileBase imageFile)
        {
            return Path.GetExtension(imageFile.FileName) == ".svg";
        }

        private static bool IsImagePng(HttpPostedFileBase imageFile)
        {
            var imageHeader = new byte[4];

            imageFile.InputStream.Read(imageHeader, 0, 4);
            var strHeader = Encoding.ASCII.GetString(imageHeader);

            return strHeader.ToLowerInvariant().EndsWith("png");
        }

        private string CheckIfAlreadyExists(string name, string template)
        {
            var imageFileName = string.Format(template, name);
            var imagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), imageFileName);

            return System.IO.File.Exists(imagePhysicalPath) ? imagePhysicalPath : null;
        }
    }
}