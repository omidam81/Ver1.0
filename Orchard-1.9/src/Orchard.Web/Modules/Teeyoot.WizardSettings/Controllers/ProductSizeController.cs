using System;
using System.Linq;
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
using Teeyoot.WizardSettings.Common;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductSizeController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductSizeRecord> _productSizeRepository;
        private readonly IRepository<SizeCodeRecord> _sizeCodeRepository;

        private dynamic Shape { get; set; }

        public ProductSizeController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ProductSizeRecord> productSizeRepository,
            IRepository<SizeCodeRecord> sizeCodeRepository,
            IShapeFactory shapeFactory)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productSizeRepository = productSizeRepository;
            _sizeCodeRepository = sizeCodeRepository;

            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var viewModel = new ProductSizeIndexViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var productSizes = _productSizeRepository.Table
                .OrderBy(s => s.SizeCodeRecord.Id)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .Fetch(s => s.SizeCodeRecord)
                .ToList();

            var pagerShape = Shape.Pager(pager).TotalItemCount(_productSizeRepository.Table.Count());

            viewModel.ProductSizes = productSizes;
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }

        public ActionResult AddProductSize()
        {
            var viewModel = new ProductSizeViewModel();
            FillProductSizeViewModelWithMetrics(viewModel);
            FillProductSizeViewModelWithSizeCodes(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddProductSize([Bind(Exclude = "Id")] ProductSizeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddProductSize");
            }

            var productSize = new ProductSizeRecord
            {
                WidthCm = ConvertSize(viewModel.WidthDimension, viewModel.Width, SizeMetricDimension.Centimetre),
                WidthInch = ConvertSize(viewModel.WidthDimension, viewModel.Width, SizeMetricDimension.Inch),
                LengthCm = ConvertSize(viewModel.LengthDimension, viewModel.Length, SizeMetricDimension.Centimetre),
                LengthInch = ConvertSize(viewModel.LengthDimension, viewModel.Length, SizeMetricDimension.Inch)
            };

            if (viewModel.Sleeve != null)
            {
                productSize.SleeveCm = ConvertSize(viewModel.SleeveDimension, viewModel.Sleeve.Value,
                    SizeMetricDimension.Centimetre);
                productSize.SleeveInch = ConvertSize(viewModel.SleeveDimension, viewModel.Sleeve.Value,
                    SizeMetricDimension.Inch);
            }

            var sizeCode = _sizeCodeRepository.Get(viewModel.SelectedSizeCode);
            productSize.SizeCodeRecord = sizeCode;

            _productSizeRepository.Create(productSize);

            _orchardServices.Notifier.Information(T("New Product Size has been added."));

            return RedirectToAction("Index");
        }

        public ActionResult EditProductSize(int productSizeId)
        {
            var productSize = _productSizeRepository.Get(productSizeId);

            var viewModel = new ProductSizeViewModel
            {
                Id = productSize.Id,
                Width = productSize.WidthCm,
                Length = productSize.LengthCm,
                Sleeve = productSize.SleeveCm,
                SelectedSizeCode = productSize.SizeCodeRecord.Id
            };

            FillProductSizeViewModelWithMetrics(viewModel);
            FillProductSizeViewModelWithSizeCodes(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProductSize(ProductSizeViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditProductSize", new {productSizeId = viewModel.Id});
            }

            var productSize = _productSizeRepository.Get(viewModel.Id);

            var sizeCode = _sizeCodeRepository.Get(viewModel.SelectedSizeCode);
            productSize.SizeCodeRecord = sizeCode;

            productSize.WidthCm = ConvertSize(viewModel.WidthDimension, viewModel.Width, SizeMetricDimension.Centimetre);
            productSize.WidthInch = ConvertSize(viewModel.WidthDimension, viewModel.Width, SizeMetricDimension.Inch);
            productSize.LengthCm = ConvertSize(viewModel.LengthDimension, viewModel.Length,
                SizeMetricDimension.Centimetre);
            productSize.LengthInch = ConvertSize(viewModel.LengthDimension, viewModel.Length, SizeMetricDimension.Inch);

            if (viewModel.Sleeve != null)
            {
                productSize.SleeveCm = ConvertSize(viewModel.SleeveDimension, viewModel.Sleeve.Value,
                    SizeMetricDimension.Centimetre);
                productSize.SleeveInch = ConvertSize(viewModel.SleeveDimension, viewModel.Sleeve.Value,
                    SizeMetricDimension.Inch);
            }

            _productSizeRepository.Update(productSize);

            _orchardServices.Notifier.Information(T("Product Size has been edited."));

            return RedirectToAction("Index");
        }

        public ActionResult DeleteProductSize(int productSizeId)
        {
            try
            {
                var productSize = _productSizeRepository.Get(productSizeId);
                _productSizeRepository.Delete(productSize);
                _productSizeRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Product Size failed: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting Product Size failed: {0}", exception.Message));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Product Size has been deleted."));
            return RedirectToAction("Index");
        }

        private void FillProductSizeViewModelWithMetrics(ProductSizeViewModel viewModel)
        {
            viewModel.SizeMetricDimensions = new SelectList(new[]
            {
                new SelectListItem
                {
                    Text = T("cm").ToString(),
                    Value = Enum.GetName(typeof (SizeMetricDimension), SizeMetricDimension.Centimetre)
                },
                new SelectListItem
                {
                    Text = T("in").ToString(),
                    Value = Enum.GetName(typeof (SizeMetricDimension), SizeMetricDimension.Inch)
                }
            }, "Value", "Text");
        }

        private void FillProductSizeViewModelWithSizeCodes(ProductSizeViewModel viewModel)
        {
            viewModel.SizeCodes = _sizeCodeRepository.Table
                .ToList();
        }

        private static double ConvertSize(
            SizeMetricDimension dimension,
            double value,
            SizeMetricDimension neededDimension)
        {
            const double inchMultiplier = 0.3937;

            if (dimension == neededDimension)
            {
                return value;
            }

            if (dimension == SizeMetricDimension.Centimetre && neededDimension == SizeMetricDimension.Inch)
            {
                return value*inchMultiplier;
            }

            if (dimension == SizeMetricDimension.Inch && neededDimension == SizeMetricDimension.Centimetre)
            {
                return value/inchMultiplier;
            }

            throw new ArgumentException();
        }
    }
}