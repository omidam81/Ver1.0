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
    public class ProductHeadlineController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<ProductHeadlineRecord> _productHeadlineRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        private dynamic Shape { get; set; }

        public ProductHeadlineController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<ProductHeadlineRecord> productHeadlineRepository,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _productHeadlineRepository = productHeadlineRepository;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var productHeadlines = _productHeadlineRepository.Table
                .Where(p => p.ProdHeadCulture == cultureUsed)
                .OrderBy(p => p.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_productHeadlineRepository.Table.Where(p => p.ProdHeadCulture == cultureUsed).Count());

            var viewModel = new ProductHeadlineIndexViewModel(productHeadlines, pagerShape);

            return View(viewModel);
        }

        public ActionResult AddProductHeadline()
        {
            var viewModel = new ProductHeadlineViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddProductHeadline([Bind(Exclude = "Id")] ProductHeadlineViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("AddProductHeadline");
            }

            var productHeadline = new ProductHeadlineRecord {Name = viewModel.Name, ProdHeadCulture = cultureUsed};
            _productHeadlineRepository.Create(productHeadline);

            _orchardServices.Notifier.Information(T("New Product Headline has been added."));
            return RedirectToAction("Index");
        }

        public ActionResult EditProductHeadline(int productHeadlineId)
        {
            var productHeadline = _productHeadlineRepository.Get(productHeadlineId);

            var viewModel = new ProductHeadlineViewModel
            {
                Id = productHeadline.Id,
                Name = productHeadline.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProductHeadline(ProductHeadlineViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    _orchardServices.Notifier.Error(T(error));
                }
                return RedirectToAction("EditProductHeadline", new {productHeadlineId = viewModel.Id});
            }

            var productHeadline = _productHeadlineRepository.Get(viewModel.Id);
            productHeadline.Name = viewModel.Name;
            _productHeadlineRepository.Update(productHeadline);

            _orchardServices.Notifier.Information(T("Product Headline has been edited."));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteProductHeadline(int productHeadlineId)
        {
            try
            {
                var productHeadline = _productHeadlineRepository.Get(productHeadlineId);
                _productHeadlineRepository.Delete(productHeadline);
                _productHeadlineRepository.Flush();
            }
            catch (Exception exception)
            {
                Logger.Error(T("Deleting Product Headline failed: {0}", exception.Message).Text);
                _orchardServices.Notifier.Error(T("Deleting Product Headline failed: {0}", exception.Message));
                return RedirectToAction("Index");
            }

            _orchardServices.Notifier.Information(T("Product Headline has been deleted."));
            return RedirectToAction("Index");
        }
    }
}