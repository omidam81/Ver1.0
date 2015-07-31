using System.Linq;
using System.Web.Mvc;
using Orchard.Data;
using Orchard.UI.Admin;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductController : Controller
    {
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IRepository<ProductColorRecord> _productColourRepository;
        private readonly IRepository<LinkProductColorRecord> _linkProductColorRepository;
        private readonly IRepository<ProductGroupRecord> _productGroupRepository;
        private readonly IRepository<LinkProductGroupRecord> _linkProductGroupRepository;
        private readonly IRepository<ProductHeadlineRecord> _productHeadlineRepository;
        private readonly IRepository<ProductImageRecord> _productImageRepository;

        public ProductController(
            IRepository<ProductRecord> productRepository,
            IRepository<ProductColorRecord> productColourRepository,
            IRepository<LinkProductColorRecord> linkProductColorRepository,
            IRepository<ProductGroupRecord> productGroupRepository,
            IRepository<LinkProductGroupRecord> linkProductGroupRepository,
            IRepository<ProductHeadlineRecord> productHeadlineRepository,
            IRepository<ProductImageRecord> productImageRepository)
        {
            _productRepository = productRepository;
            _productColourRepository = productColourRepository;
            _linkProductColorRepository = linkProductColorRepository;
            _productGroupRepository = productGroupRepository;
            _linkProductGroupRepository = linkProductGroupRepository;
            _productHeadlineRepository = productHeadlineRepository;
            _productImageRepository = productImageRepository;
        }

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
    }
}