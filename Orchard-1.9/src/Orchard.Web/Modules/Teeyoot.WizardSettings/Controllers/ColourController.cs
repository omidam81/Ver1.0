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
using Teeyoot.WizardSettings.Models;
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ColourController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductColorRecord> _productColourRepository;

        private dynamic Shape { get; set; }

        public ColourController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ProductColorRecord> productColourRepository,
            IShapeFactory shapeFactory)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productColourRepository = productColourRepository;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
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
                    .OrderBy(p => p.Name)
                    .Skip(pager.GetStartIndex())
                    .Take(pager.PageSize);

                var pagerShape = Shape.Pager(pager).TotalItemCount(_productColourRepository.Table.Count());

                viewModel.Colors = productColours;
                viewModel.Pager = pagerShape;
            }

            return View(viewModel);
        }

        public ActionResult AddProductColour()
        {
            var viewModel = new ProductColourViewModel();

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
                Value = viewModel.Value
            };

            _productColourRepository.Create(productColour);

            _orchardServices.Notifier.Information(T("New Product Colour has been added."));
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
                _orchardServices.Notifier.Error(T("Deleting Product Colour failed: {0}", exception.Message));
                return RedirectToAction("Index",
                    new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
            }

            _orchardServices.Notifier.Information(T("Product Colour has been deleted."));
            return RedirectToAction("Index",
                new {chooseColourFor = Enum.GetName(typeof (ChooseColourFor), ChooseColourFor.Product)});
        }
    }
}