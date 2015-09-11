using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ArtworkController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ArtRecord> _artRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        private const string ArtworksImagesRelativePath = "~/Modules/Teeyoot.Module/Content/vector";
        private const string ArtworkSvgImageFileNameTemplate = "{0}.svg";
        private const string ArtworkPngImageFileNameTemplate = "{0}.png";
        private const string ArtworkFileNameTemplate = "{0}_{1}";

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ArtworkController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ArtRecord> artRepository,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _artRepository = artRepository;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var viewModel = new ArtworkIndexViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            viewModel.ArtworksImagesRelativePath = ArtworksImagesRelativePath;

            viewModel.Arts = _artRepository.Table
                .Where(a => a.ArtCulture == cultureUsed)
                .OrderBy(a => a.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_artRepository.Table.Where(a => a.ArtCulture == cultureUsed).Count());
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

            // If art file with the name already exist we are adding GUID ending
            var fileName = viewModel.Name;
            var existingArtwork = _artRepository.Table
                .FirstOrDefault(a => a.ArtCulture == cultureUsed && a.Name == viewModel.Name);

            if (existingArtwork != null)
            {
                fileName = string.Format(ArtworkFileNameTemplate, viewModel.Name, Guid.NewGuid());
            }

            var success = SaveArtworkImages(
                viewModel.ArtworkSvgImage,
                viewModel.ArtworkPngImage,
                fileName);

            if (!success)
            {
                return RedirectToAction("AddArtwork");
            }

            var art = new ArtRecord
            {
                Name = viewModel.Name,
                FileName = string.Format(ArtworkSvgImageFileNameTemplate, viewModel.Name),
                ArtCulture = cultureUsed
            };
            _artRepository.Create(art);

            _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been added", viewModel.Name));
            return RedirectToAction("Index");
        }

        public ActionResult EditArtwork(int artworkId)
        {
            var artwork = _artRepository.Get(artworkId);

            var viewModel = new ArtworkViewModel
            {
                Id = artwork.Id,
                Name = artwork.Name
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

            var artwork = _artRepository.Get(viewModel.Id);

            // If art file with the name already exist we are adding GUID ending
            var fileName = viewModel.Name;
            var existingArtwork = _artRepository.Table
                .FirstOrDefault(a => a.Name == viewModel.Name && a != artwork);

            if (existingArtwork != null)
            {
                fileName = string.Format(ArtworkFileNameTemplate, viewModel.Name, Guid.NewGuid());
            }

            var success = UpdateArtworkImages(
                viewModel.ArtworkSvgImage,
                viewModel.ArtworkPngImage,
                fileName,
                artwork.Name);

            if (!success)
            {
                RedirectToAction("EditArtwork", new {artworkId = artwork.Id});
            }

            artwork.Name = viewModel.Name;
            artwork.FileName = string.Format(ArtworkSvgImageFileNameTemplate, viewModel.Name);
            _artRepository.Update(artwork);

            _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been edited.", artwork.Name));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteArtwork(int artworkId)
        {
            var artwork = _artRepository.Get(artworkId);

            try
            {
                var svgImageFileName = string.Format(ArtworkSvgImageFileNameTemplate, artwork.Name);
                var svgImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), svgImageFileName);

                System.IO.File.Delete(svgImagePhysicalPath);

                var pngImageFileName = string.Format(ArtworkPngImageFileNameTemplate, artwork.Name);
                var pngImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), pngImageFileName);

                System.IO.File.Delete(pngImagePhysicalPath);

                _artRepository.Delete(artwork);
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting artwork \"{0}\" failed: {1}", artwork.Name, exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting artwork \"{0}\" failed.", artwork.Name));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been deleted.", artwork.Name));
            return RedirectToAction("Index");
        }

        private bool SaveArtworkImages(
            HttpPostedFileBase svgImageFile,
            HttpPostedFileBase pngImageFile,
            string fileName)
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