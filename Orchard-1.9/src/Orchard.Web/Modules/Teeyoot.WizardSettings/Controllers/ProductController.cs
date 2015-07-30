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

        public ProductController(
            IRepository<ProductRecord> productRepository,
            IRepository<ProductColorRecord> productColourRepository,
            IRepository<LinkProductColorRecord> linkProductColorRepository)
        {
            _productRepository = productRepository;
            _productColourRepository = productColourRepository;
            _linkProductColorRepository = linkProductColorRepository;
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
            }

            FillProductViewModelWithColours(productViewModel, product);

            return View(productViewModel);
        }

        [HttpPost]
        public ActionResult EditProduct(ProductViewModel viewModel)
        {
            var product = viewModel.Id == null ? new ProductRecord() : _productRepository.Get(viewModel.Id.Value);

            product.Name = viewModel.Name;

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
    }
}