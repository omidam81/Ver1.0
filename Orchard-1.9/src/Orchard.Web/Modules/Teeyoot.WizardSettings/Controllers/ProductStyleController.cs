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
using Teeyoot.WizardSettings.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductStyleController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductGroupRecord> _productStyleRepository;

        private dynamic Shape { get; set; }

        public ProductStyleController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ProductGroupRecord> productStyleRepository,
            IShapeFactory shapeFactory)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productStyleRepository = productStyleRepository;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var productHeadlines = _productStyleRepository.Table
                .OrderBy(p => p.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_productStyleRepository.Table.Count());

            var viewModel = new ProductStyleIndexViewModel(productHeadlines, pagerShape);

            return View(viewModel);
        }

        public ActionResult AddProductStyle()
        {
            var viewModel = new ProductStyleViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddProductStyle([Bind(Exclude = "Id")] ProductStyleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddProductStyle");
            }

            var productStyle = new ProductGroupRecord {Name = viewModel.Name};
            _productStyleRepository.Create(productStyle);

            _orchardServices.Notifier.Information(T("New Product Style has been added."));
            return RedirectToAction("Index");
        }

        public ActionResult EditProductStyle(int productStyleId)
        {
            var productStyle = _productStyleRepository.Get(productStyleId);

            var viewModel = new ProductStyleViewModel
            {
                Id = productStyle.Id,
                Name = productStyle.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProductStyle(ProductStyleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditProductStyle", new {productStyleId = viewModel.Id});
            }

            var productStyle = _productStyleRepository.Get(viewModel.Id);
            productStyle.Name = viewModel.Name;
            _productStyleRepository.Update(productStyle);

            _orchardServices.Notifier.Information(T("Product Style has been edited."));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteProductStyle(int productStyleId)
        {
            try
            {
                var productStyle = _productStyleRepository.Get(productStyleId);
                _productStyleRepository.Delete(productStyle);
                _productStyleRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Product Style failed: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting Product Style failed: {0}", exception.Message));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Product Style has been deleted."));
            return RedirectToAction("Index");
        }
    }
}