using System.Web.Mvc;
using Orchard.Data;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.Common;

namespace Teeyoot.WizardSettings.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IRepository<ProductRecord> _productRepository;

        public ProductImageController(
            IRepository<ProductRecord> productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public JsonResult GetImage(int productId)
        {
            var product = _productRepository.Get(productId);
            var productImage = product.ProductImageRecord;

            var response = new GetImageResponse
            {
                Ppi = productImage.Ppi,

                // Front Image
                PrintableFrontLeft = productImage.PrintableFrontLeft,
                PrintableFrontTop = productImage.PrintableFrontTop,
                PrintableFrontWidth = productImage.PrintableFrontWidth,
                PrintableFrontHeight = productImage.PrintableFrontHeight,

                ChestLineFront = productImage.ChestLineFront,

                // Back Image
                PrintableBackLeft = productImage.PrintableBackLeft,
                PrintableBackTop = productImage.PrintableBackTop,
                PrintableBackWidth = productImage.PrintableBackWidth,
                PrintableBackHeight = productImage.PrintableBackHeight,

                ChestLineBack = productImage.ChestLineBack
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditImage(EditImageRequest request)
        {
            var product = _productRepository.Get(request.ProductId);
            var productImage = product.ProductImageRecord;

            productImage.Ppi = request.Ppi;

            // Front Image
            productImage.PrintableFrontLeft = request.PrintableFrontLeft;
            productImage.PrintableFrontTop = request.PrintableFrontTop;
            productImage.PrintableFrontWidth = request.PrintableFrontWidth;
            productImage.PrintableFrontHeight = request.PrintableFrontHeight;

            productImage.ChestLineFront = request.ChestLineFront;

            // Back Image
            productImage.PrintableBackLeft = request.PrintableBackLeft;
            productImage.PrintableBackTop = request.PrintableBackTop;
            productImage.PrintableBackWidth = request.PrintableBackWidth;
            productImage.PrintableBackHeight = request.PrintableBackHeight;

            productImage.ChestLineBack = request.ChestLineBack;

            return Json(true);
        }
    }
}