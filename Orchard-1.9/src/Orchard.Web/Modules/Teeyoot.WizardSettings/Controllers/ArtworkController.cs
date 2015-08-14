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
        private const string ArtworkImageFileNameTemplate = "{0}.png";

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

            var success = SaveArtworkImage(viewModel.ArtworkImage, viewModel.Name);
            if (success)
            {
                _orchardServices.Notifier.Information(T("Artwork \"{0}\" has been added.", viewModel.Name));
            }

            return RedirectToAction("Index");
        }

        public ActionResult EditArtwork(string name)
        {
            var artworkName = Path.GetFileNameWithoutExtension(CheckIfAlreadyExists(name));

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

            var imagePhysicalPath = CheckIfAlreadyExists(viewModel.CurrentName);

            if (viewModel.ArtworkImage != null)
            {
                System.IO.File.Delete(imagePhysicalPath);
                SaveArtworkImage(viewModel.ArtworkImage, viewModel.Name);
            }
            else
            {
                var existingImagePhysicalPath = CheckIfAlreadyExists(viewModel.Name);
                if (existingImagePhysicalPath == null)
                {
                    var newImageFileName = string.Format(ArtworkImageFileNameTemplate, viewModel.Name);
                    var newImagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), newImageFileName);

                    System.IO.File.Move(imagePhysicalPath, newImagePhysicalPath);
                }
                else if (Path.GetFileNameWithoutExtension(existingImagePhysicalPath) != viewModel.Name)
                {
                    _orchardServices.Notifier.Error(T("Image with the same name already exist"));
                    return RedirectToAction("EditArtwork", new {name = viewModel.CurrentName});
                }
            }

            _orchardServices.Notifier.Information(T("Artwork has been edited."));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteArtwork(string name)
        {
            try
            {
                var imageFileName = string.Format(ArtworkImageFileNameTemplate, name);
                var imagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), imageFileName);

                System.IO.File.Delete(imagePhysicalPath);
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

        private string CheckIfAlreadyExists(string name)
        {
            var imageFileName = string.Format(ArtworkImageFileNameTemplate, name);
            var imagePhysicalPath = Path.Combine(Server.MapPath(ArtworksImagesRelativePath), imageFileName);

            return System.IO.File.Exists(imagePhysicalPath) ? imagePhysicalPath : null;
        }
    }
}