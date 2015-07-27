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

        public ActionResult Index(ChooseColourFor? chooseColourFor)
        {
            var viewModel = new ColourIndexViewModel(chooseColourFor);

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

        public ActionResult ProductColourList(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var productColours = _productColourRepository.Table
                .OrderBy(p => p.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_productColourRepository.Table.Count());

            var viewModel = new ProductColourListViewModel(productColours, pagerShape);

            return View(viewModel);
        }
    }
}