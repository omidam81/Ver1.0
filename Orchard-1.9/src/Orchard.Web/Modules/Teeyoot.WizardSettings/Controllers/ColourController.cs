using System;
using System.Drawing;
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
    public class ColourController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductColorRecord> _productColourRepository;
        private readonly IRepository<SwatchRecord> _swatchColourRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        private dynamic Shape { get; set; }

        public ColourController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ProductColorRecord> productColourRepository,
            IRepository<SwatchRecord> swatchRecordRepository,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productColourRepository = productColourRepository;
            _swatchColourRepository = swatchRecordRepository;

            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(ChooseColourFor? chooseColourFor, PagerParameters pagerParameters)
        {
            var viewModel = new ColourIndexViewModel(chooseColourFor);

            if (chooseColourFor == null)
            {
                return View(viewModel);
            }

            if (chooseColourFor.Value == ChooseColourFor.Product)
            {
                var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

                var productColours = _productColourRepository.Table
                    .Where(p => p.ProdColorCulture == cultureUsed)
                    .OrderBy(p => p.Name)
                    .Skip(pager.GetStartIndex())
                    .Take(pager.PageSize);

                var pagerShape = Shape.Pager(pager).TotalItemCount(_productColourRepository.Table.Where(p => p.ProdColorCulture == cultureUsed).Count());

                viewModel.Colors = productColours;
                viewModel.Pager = pagerShape;
            }
            else if (chooseColourFor == ChooseColourFor.Swatch)
            {
                var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

                var swatchColours = _swatchColourRepository.Table
                    .Where(p => p.SwatchCulture == cultureUsed)
                    .OrderBy(p => p.Name)
                    .Skip(pager.GetStartIndex())
                    .Take(pager.PageSize)
                    .Select(s => new SwatchColourViewModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = ColorTranslator.ToHtml(Color.FromArgb(s.Red, s.Green, s.Blue)),
                        InStock = s.InStock
                    });

                var pagerShape = Shape.Pager(pager).TotalItemCount(_swatchColourRepository.Table.Where(s => s.SwatchCulture == cultureUsed).Count());

                viewModel.Colors = swatchColours;
                viewModel.Pager = pagerShape;
            }

            return View(viewModel);
        }

        public ActionResult AddProductColour(string returnUrl)
        {
            var viewModel = new ProductColourViewModel {ReturnUrl = returnUrl};

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddProductColour([Bind(Exclude = "Id")] ProductColourViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddProductColour");
            }

            var productColour = new ProductColorRecord
            {
                Name = viewModel.Name,
                Value = viewModel.Value,
                ProdColorCulture = cultureUsed
            };

            _productColourRepository.Create(productColour);

            _orchardServices.Notifier.Information(T("New Product Colour \"{0}\" has been added.", productColour.Name));

            if (!string.IsNullOrEmpty(viewModel.ReturnUrl))
            {
                return Redirect(viewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Colour",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
        }

        public ActionResult EditProductColour(int productColourId)
        {
            var productColour = _productColourRepository.Get(productColourId);

            var viewModel = new ProductColourViewModel
            {
                Id = productColour.Id,
                Name = productColour.Name,
                Value = productColour.Value
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProductColour(ProductColourViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditProductColour", new {productColourId = viewModel.Id});
            }

            var productColour = _productColourRepository.Get(viewModel.Id);
            productColour.Name = viewModel.Name;
            productColour.Value = viewModel.Value;
            _productColourRepository.Update(productColour);

            _orchardServices.Notifier.Information(T("Product Colour has been edited."));
            return RedirectToAction("Index", "Colour",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
        }

        public ActionResult DeleteProductColour(int productColourId)
        {
            try
            {
                var productColour = _productColourRepository.Get(productColourId);
                _productColourRepository.Delete(productColour);
                _productColourRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Product Colour failed: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(
                    T("Deleting Product Colour failed. There are references to another objects."));
                return RedirectToAction("Index",
                    new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
            }

            _orchardServices.Notifier.Information(T("Product Colour has been deleted."));
            return RedirectToAction("Index",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
        }

        public ActionResult AddSwatchColour()
        {
            var viewModel = new SwatchColourViewModel {InStock = true};

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddSwatchColour([Bind(Exclude = "Id")] SwatchColourViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddSwatchColour");
            }

            var rgbColor = ColorTranslator.FromHtml(viewModel.Value);

            var swatchColour = new SwatchRecord
            {
                Name = viewModel.Name,
                Red = rgbColor.R,
                Green = rgbColor.G,
                Blue = rgbColor.B,
                InStock = viewModel.InStock,
                SwatchCulture = cultureUsed
            };

            _swatchColourRepository.Create(swatchColour);

            _orchardServices.Notifier.Information(T("New Swatch Colour has been added."));
            return RedirectToAction("Index", "Colour",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Swatch)});
        }

        public ActionResult EditSwatchColour(int swatchColourId)
        {
            var swatchColour = _swatchColourRepository.Get(swatchColourId);

            var viewModel = new SwatchColourViewModel
            {
                Id = swatchColour.Id,
                Name = swatchColour.Name,
                Value = ColorTranslator.ToHtml(Color.FromArgb(swatchColour.Red, swatchColour.Green, swatchColour.Blue)),
                InStock = swatchColour.InStock
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditSwatchColour(SwatchColourViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditSwatchColour", new {swatchColourId = viewModel.Id});
            }

            var rgbColor = ColorTranslator.FromHtml(viewModel.Value);

            var swatchColour = _swatchColourRepository.Get(viewModel.Id);

            swatchColour.Name = viewModel.Name;
            swatchColour.Red = rgbColor.R;
            swatchColour.Green = rgbColor.G;
            swatchColour.Blue = rgbColor.B;
            swatchColour.InStock = viewModel.InStock;

            _swatchColourRepository.Update(swatchColour);

            _orchardServices.Notifier.Information(T("Swatch Colour has been edited."));
            return RedirectToAction("Index", "Colour",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Swatch)});
        }

        public ActionResult DeleteSwatchColour(int swatchColourId)
        {
            try
            {
                var swatchColour = _swatchColourRepository.Get(swatchColourId);
                _swatchColourRepository.Delete(swatchColour);
                _swatchColourRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Swatch Colour failed: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(
                    T("Deleting Swatch Colour failed. There are references to another objects."));
                return RedirectToAction("Index",
                    new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Swatch)});
            }

            _orchardServices.Notifier.Information(T("Swatch Colour has been deleted."));
            return RedirectToAction("Index",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Swatch)});
        }
    }
}