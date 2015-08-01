using System;
using System.Drawing;
using System.Drawing.Imaging;
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
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.Common;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IRepository<ProductColorRecord> _productColourRepository;
        private readonly IRepository<LinkProductColorRecord> _linkProductColorRepository;
        private readonly IRepository<ProductGroupRecord> _productGroupRepository;
        private readonly IRepository<LinkProductGroupRecord> _linkProductGroupRepository;
        private readonly IRepository<ProductHeadlineRecord> _productHeadlineRepository;
        private readonly IRepository<ProductImageRecord> _productImageRepository;
        private readonly IRepository<ProductSizeRecord> _productSizeRepository;
        private readonly IRepository<LinkProductSizeRecord> _linkProductSizeRepository;

        private readonly IimageHelper _imageHelper;

        private const int ProductImageWidth = 530;
        private const int ProductImageHeight = 630;
        private const int ProductImagePpi = 18;

        private const int ProductImageFrontSmallWidth = 212;
        private const int ProductImageFrontSmallHeight = 252;

        private const string ProductImagesRelativePath = "~/Modules/Teeyoot.Module/Content/images";

        private const string ProductImageFrontFileNameTemplate = "product_type_{0}_front.png";
        private const string ProductImageBackFileNameTemplate = "product_type_{0}_back.png";

        private const string ProductImageFrontSmallFileNameTemplate = "product_type_{0}_front_small.png";

        private dynamic Shape { get; set; }

        public ProductController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IRepository<ProductRecord> productRepository,
            IRepository<ProductColorRecord> productColourRepository,
            IRepository<LinkProductColorRecord> linkProductColorRepository,
            IRepository<ProductGroupRecord> productGroupRepository,
            IRepository<LinkProductGroupRecord> linkProductGroupRepository,
            IRepository<ProductHeadlineRecord> productHeadlineRepository,
            IRepository<ProductImageRecord> productImageRepository,
            IRepository<ProductSizeRecord> productSizeRepository,
            IRepository<LinkProductSizeRecord> linkProductSizeRepository,
            IimageHelper imageHelper)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;

            _productRepository = productRepository;
            _productColourRepository = productColourRepository;
            _linkProductColorRepository = linkProductColorRepository;
            _productGroupRepository = productGroupRepository;
            _linkProductGroupRepository = linkProductGroupRepository;
            _productHeadlineRepository = productHeadlineRepository;
            _productImageRepository = productImageRepository;
            _productSizeRepository = productSizeRepository;
            _linkProductSizeRepository = linkProductSizeRepository;

            _imageHelper = imageHelper;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            Shape = shapeFactory;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var products = _productRepository.Table
                .OrderBy(p => p.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_productRepository.Table.Count());

            var viewModel = new ProductIndexViewModel(products, pagerShape);

            return View(viewModel);
        }

        public ActionResult EditProduct(int? productId)
        {
            var viewModel = new ProductViewModel(productId);

            ProductRecord product = null;
            if (productId != null)
            {
                product = _productRepository.Get(productId.Value);
            }

            if (product != null)
            {
                viewModel.Name = product.Name;
                viewModel.SelectedProductHeadline = product.ProductHeadlineRecord.Id;

                viewModel.ProductImageFrontFileName = GetProductImageFileName(product,
                    ProductImageFrontFileNameTemplate);
                viewModel.ProductImageBackFileName = GetProductImageFileName(product,
                    ProductImageBackFileNameTemplate);

                viewModel.Materials = product.Materials;
                viewModel.Details = product.Details;
            }

            FillProductViewModelWithHeadlines(viewModel);
            FillProductViewModelWithGroups(viewModel, product);
            FillProductViewModelWithColours(viewModel, product);
            FillProductViewModelWithSizes(viewModel, product);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductViewModel viewModel)
        {
            var product = viewModel.Id == null
                ? new ProductRecord()
                : _productRepository.Get(viewModel.Id.Value);

            product.Name = viewModel.Name;

            var productHeadline = _productHeadlineRepository.Get(viewModel.SelectedProductHeadline);
            product.ProductHeadlineRecord = productHeadline;

            product.Materials = viewModel.Materials;
            product.Details = viewModel.Details;

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

            var linkProductSizes = _linkProductSizeRepository.Table
                .Where(it => it.ProductRecord == product)
                .ToList();

            foreach (var linkProductSize in linkProductSizes)
            {
                _linkProductSizeRepository.Delete(linkProductSize);
            }

            foreach (var productSizeId in viewModel.SelectedProductSizes)
            {
                var productSize = _productSizeRepository.Get(productSizeId);

                var linkProductSize = new LinkProductSizeRecord
                {
                    ProductRecord = product,
                    ProductSizeRecord = productSize
                };

                _linkProductSizeRepository.Create(linkProductSize);
            }

            var frontImageSavingResult = SaveProductFrontImage(viewModel.ProductImageFront, product);
            var backImageSavingResult = SaveProductBackImage(viewModel.ProductImageBack, product);

            if (frontImageSavingResult != null)
            {
                FillProductImageWith(product.ProductImageRecord,
                    frontImageSavingResult.Width,
                    frontImageSavingResult.Height,
                    ProductImagePpi);
            }
            else if (backImageSavingResult != null)
            {
                FillProductImageWith(product.ProductImageRecord,
                    backImageSavingResult.Width,
                    backImageSavingResult.Height,
                    ProductImagePpi);
            }

            return RedirectToAction("EditProduct", new {productId = product.Id});
        }

        public ActionResult DeleteProduct(int productId)
        {
            var product = _productRepository.Get(productId);

            try
            {
                // Removing ProductImage
                _productImageRepository.Delete(product.ProductImageRecord);

                // Removing ProductGroups
                var productGroups = _linkProductGroupRepository.Table
                    .Where(g => g.ProductRecord == product)
                    .ToList();

                foreach (var productGroup in productGroups)
                {
                    _linkProductGroupRepository.Delete(productGroup);
                }

                // Removing ProductColours
                var productColours = _linkProductColorRepository.Table
                    .Where(c => c.ProductRecord == product)
                    .ToList();

                foreach (var productColour in productColours)
                {
                    _linkProductColorRepository.Delete(productColour);
                }

                // Removing ProductSizes
                var productSizes = _linkProductSizeRepository.Table
                    .Where(s => s.ProductRecord == product)
                    .ToList();

                foreach (var productSize in productSizes)
                {
                    _linkProductSizeRepository.Delete(productSize);
                }

                _productRepository.Delete(product);
                _productRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Product \"{0}\" failed: {1}", product.Name, exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting Product \"{0}\" failed: {1}", product.Name,
                    exception.Message));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Product \"{0}\" has been deleted.", product.Name));
            return RedirectToAction("Index");
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

        private void FillProductViewModelWithSizes(ProductViewModel viewModel, ProductRecord product)
        {
            viewModel.ProductSizes = _productSizeRepository.Table
                .Fetch(s => s.SizeCodeRecord)
                .Select(s => new ProductSizeItemViewModel
                {
                    Id = s.Id,
                    LengthCm = s.LengthCm,
                    WidthCm = s.WidthCm,
                    SleeveCm = s.SleeveCm,
                    LengthInch = s.LengthInch,
                    WidthInch = s.WidthInch,
                    SleeveInch = s.SleeveInch,
                    SizeCodeId = s.SizeCodeRecord.Id,
                    SizeCodeName = s.SizeCodeRecord.Name
                })
                .ToList();

            if (product == null)
            {
                return;
            }

            var selectedProductSizeIds = _linkProductSizeRepository.Table
                .Where(it => it.ProductRecord == product)
                .Select(it => it.ProductSizeRecord.Id)
                .ToList();

            viewModel.ProductSizes.ToList().ForEach(s =>
            {
                if (selectedProductSizeIds.Contains(s.Id))
                {
                    s.Selected = true;
                }
            });
        }

        private ProductImageSavingResult SaveProductFrontImage(HttpPostedFileBase imageFile, ProductRecord product)
        {
            if (imageFile == null)
            {
                return null;
            }

            if (!IsImagePng(imageFile))
            {
                _orchardServices.Notifier.Error(
                    T("Front Image file should be *.png.",
                        ProductImageWidth,
                        ProductImageHeight));

                return null;
            }

            using (var image = Image.FromStream(imageFile.InputStream, true, true))
            {
                if (image.Width != ProductImageWidth || image.Height != ProductImageHeight)
                {
                    _orchardServices.Notifier.Error(
                        T("Front Image should be {0}x{1}.",
                            ProductImageWidth,
                            ProductImageHeight));

                    return null;
                }

                var imageFileName = string.Format(ProductImageFrontFileNameTemplate, product.Id);
                var imagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), imageFileName);

                image.Save(imagePhysicalPath, ImageFormat.Png);

                var smallImageBitmap = _imageHelper.ResizeImage(
                    image,
                    ProductImageFrontSmallWidth,
                    ProductImageFrontSmallHeight);

                var smallImageFileName = string.Format(ProductImageFrontSmallFileNameTemplate, product.Id);
                var smallImagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), smallImageFileName);

                smallImageBitmap.Save(smallImagePhysicalPath, ImageFormat.Png);

                return new ProductImageSavingResult
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }
        }

        private ProductImageSavingResult SaveProductBackImage(HttpPostedFileBase imageFile, ProductRecord product)
        {
            if (imageFile == null)
            {
                return null;
            }

            if (!IsImagePng(imageFile))
            {
                _orchardServices.Notifier.Error(
                    T("Back Image file should be *.png.",
                        ProductImageWidth,
                        ProductImageHeight));

                return null;
            }

            using (var image = Image.FromStream(imageFile.InputStream, true, true))
            {
                if (image.Width != ProductImageWidth || image.Height != ProductImageHeight)
                {
                    _orchardServices.Notifier.Error(
                        T("Back Image should be {0}x{1}.",
                            ProductImageWidth,
                            ProductImageHeight));

                    return null;
                }

                var imageFileName = string.Format(ProductImageBackFileNameTemplate, product.Id);
                var imagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), imageFileName);

                image.Save(imagePhysicalPath, ImageFormat.Png);

                return new ProductImageSavingResult
                {
                    Width = image.Width,
                    Height = image.Height
                };
            }
        }

        private static void FillProductImageWith(ProductImageRecord productImageRecord, int width, int height, int ppi)
        {
            productImageRecord.Width = width;
            productImageRecord.Height = height;
            productImageRecord.Ppi = ppi;
        }

        private string GetProductImageFileName(ProductRecord product, string productImageFileNameTemplate)
        {
            var imageFileName = string.Format(productImageFileNameTemplate, product.Id);
            var imagePhysicalPath = Path.Combine(Server.MapPath(ProductImagesRelativePath), imageFileName);

            return System.IO.File.Exists(imagePhysicalPath) ? imageFileName : null;
        }

        private static bool IsImagePng(HttpPostedFileBase imageFile)
        {
            var imageHeader = new byte[4];

            imageFile.InputStream.Read(imageHeader, 0, 4);
            var strHeader = Encoding.ASCII.GetString(imageHeader);

            return strHeader.ToLowerInvariant().EndsWith("png");
        }
    }
}
