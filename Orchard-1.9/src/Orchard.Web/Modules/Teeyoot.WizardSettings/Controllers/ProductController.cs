using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IRepository<ProductColorRecord> _productColourRepository;
        private readonly IRepository<LinkProductColorRecord> _linkProductColorRepository;
        private readonly IRepository<ProductGroupRecord> _productGroupRepository;
        private readonly IRepository<LinkProductGroupRecord> _linkProductGroupRepository;
        private readonly IRepository<ProductHeadlineRecord> _productHeadlineRepository;
        private readonly IRepository<ProductImageRecord> _productImageRepository;

        private readonly IimageHelper _imageHelper;

        private const int ProductImageWidth = 530;
        private const int ProductImageHeight = 630;

        private const int ProductImageFrontSmallWidth = 212;
        private const int ProductImageFrontSmallHeight = 252;

        private const string ProductImagesRelativePath = "~/Modules/Teeyoot.Module/Content/images";

        private const string ProductImageFrontFilenameTemplate = "product_type_{0}_front.png";
        private const string ProductImageFrontSmallFilenameTemplate = "product_type_{0}_front_small.png";
        private const string ProductImageBackFilenameTemplate = "product_type_{0}_back.png";

        public ProductController(
            IOrchardServices orchardServices,
            IRepository<ProductRecord> productRepository,
            IRepository<ProductColorRecord> productColourRepository,
            IRepository<LinkProductColorRecord> linkProductColorRepository,
            IRepository<ProductGroupRecord> productGroupRepository,
            IRepository<LinkProductGroupRecord> linkProductGroupRepository,
            IRepository<ProductHeadlineRecord> productHeadlineRepository,
            IRepository<ProductImageRecord> productImageRepository,
            IimageHelper imageHelper)
        {
            _orchardServices = orchardServices;
            _productRepository = productRepository;
            _productColourRepository = productColourRepository;
            _linkProductColorRepository = linkProductColorRepository;
            _productGroupRepository = productGroupRepository;
            _linkProductGroupRepository = linkProductGroupRepository;
            _productHeadlineRepository = productHeadlineRepository;
            _productImageRepository = productImageRepository;
            _imageHelper = imageHelper;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EditProduct(int? productId)
        {
            var productViewModel = new ProductViewModel(productId);

            ProductRecord product = null;
            if (productId != null)
            {
                product = _productRepository.Get(productId.Value);
            }

            if (product != null)
            {
                productViewModel.Name = product.Name;
                productViewModel.SelectedProductHeadline = product.ProductHeadlineRecord.Id;
            }

            FillProductViewModelWithColours(productViewModel, product);
            FillProductViewModelWithGroups(productViewModel, product);
            FillProductViewModelWithHeadlines(productViewModel);

            return View(productViewModel);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductViewModel viewModel)
        {
            var product = viewModel.Id == null ? new ProductRecord() : _productRepository.Get(viewModel.Id.Value);

            product.Name = viewModel.Name;

            var productHeadline = _productHeadlineRepository.Get(viewModel.SelectedProductHeadline);
            product.ProductHeadlineRecord = productHeadline;

            if (product.ProductImageRecord == null)
            {
                var productImage = new ProductImageRecord();
                _productImageRepository.Create(productImage);

                product.ProductImageRecord = productImage;
            }

            foreach (var linkProductColour in product.ColorsAvailable)
            {
                _linkProductColorRepository.Delete(linkProductColour);
            }

            foreach (var productColourItem in viewModel.SelectedProductColours)
            {
                var productColour = _productColourRepository.Get(productColourItem.ProductColourId);

                var linkProductColour = new LinkProductColorRecord
                {
                    ProductRecord = product,
                    ProductColorRecord = productColour,
                    BaseCost = productColourItem.BaseCost
                };

                _linkProductColorRepository.Create(linkProductColour);
            }

            if (viewModel.Id == null)
            {
                _productRepository.Create(product);
            }
            else
            {
                _productRepository.Update(product);
            }

            var linkProductGroups = _linkProductGroupRepository.Table
                .Where(it => it.ProductRecord == product)
                .ToList();

            foreach (var linkProductGroup in linkProductGroups)
            {
                _linkProductGroupRepository.Delete(linkProductGroup);
            }

            foreach (var productGroupId in viewModel.SelectedProductGroups)
            {
                var productGroup = _productGroupRepository.Get(productGroupId);

                var linkProductGroup = new LinkProductGroupRecord
                {
                    ProductRecord = product,
                    ProductGroupRecord = productGroup
                };

                _linkProductGroupRepository.Create(linkProductGroup);
            }

            SaveProductFrontImage(viewModel.ProductImageFront, product);

            return RedirectToAction("EditProduct", new {productId = product.Id});
        }

        private void FillProductViewModelWithColours(ProductViewModel viewModel, ProductRecord product)
        {
            viewModel.ProductColours = _productColourRepository.Table
                .OrderBy(c => c.Name)
                .Select(c => new ProductColourItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    HexValue = c.Value
                })
                .ToList();

            if (product == null)
            {
                return;
            }

            var selectedProductColourIds = _linkProductColorRepository.Table
                .Where(it => it.ProductRecord == product)
                .Select(it => it.ProductColorRecord.Id)
                .ToList();

            viewModel.ProductColours.ToList().ForEach(c =>
            {
                if (selectedProductColourIds.Contains(c.Id))
                {
                    c.Selected = true;
                }
            });

            viewModel.SelectedProductColours = _linkProductColorRepository.Table
                .Where(it => it.ProductRecord == product)
                .Fetch(it => it.ProductColorRecord)
                .Select(it => new ProductColourItemViewModel
                {
                    Id = it.Id,
                    ProductColourId = it.ProductColorRecord.Id,
                    Name = it.ProductColorRecord.Name,
                    HexValue = it.ProductColorRecord.Value,
                    BaseCost = it.BaseCost
                })
                .ToList();
        }

        private void FillProductViewModelWithGroups(ProductViewModel viewModel, ProductRecord product)
        {
            viewModel.ProductGroups = _productGroupRepository.Table
                .OrderBy(g => g.Name)
                .Select(g => new ProductGroupItemViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
                .ToList();

            var selectedProductGroupIds = _linkProductGroupRepository.Table
                .Where(it => it.ProductRecord == product)
                .Select(it => it.ProductGroupRecord.Id)
                .ToList();

            viewModel.ProductGroups.ToList().ForEach(g =>
            {
                if (selectedProductGroupIds.Contains(g.Id))
                {
                    g.Selected = true;
                }
            });
        }

        private void FillProductViewModelWithHeadlines(ProductViewModel viewModel)
        {
            viewModel.ProductHeadlines = _productHeadlineRepository.Table
                .OrderBy(h => h.Name)
                .Select(h => new ProductHeadlineViewModel
                {
                    Id = h.Id,
                    Name = h.Name
                })
                .ToList();
        }

        private void SaveProductFrontImage(HttpPostedFileBase imageFile, ProductRecord product)
        {
            if (imageFile == null)
            {
                return;
            }

            using (var image = Image.FromStream(imageFile.InputStream, true, true))
            {
                if (image.Width != ProductImageWidth || image.Height != ProductImageHeight)
                {
                    _orchardServices.Notifier.Error(T("Front Image should be {0}x{1}.", ProductImageWidth,
                        ProductImageHeight));
                    return;
                }

                var imageFilename = string.Format(ProductImageFrontFilenameTemplate, product.Id);
                var imagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), imageFilename);

                image.Save(imagePhysicalPath, ImageFormat.Png);

                var smallImageBitmap = _imageHelper.ResizeImage(
                    image,
                    ProductImageFrontSmallWidth,
                    ProductImageFrontSmallHeight);

                var smallImageFilename = string.Format(ProductImageFrontSmallFilenameTemplate, product.Id);
                var smallImagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), smallImageFilename);

                smallImageBitmap.Save(smallImagePhysicalPath, ImageFormat.Png);
            }
        }
    }
}
