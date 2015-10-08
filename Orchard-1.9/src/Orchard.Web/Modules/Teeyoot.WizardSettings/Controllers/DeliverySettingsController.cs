using System;
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
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.ViewModels;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class DeliverySettingsController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IDeliverySettingsService _deliverySettingService;
        private readonly IRepository<DeliverySettingRecord> _deliverySettingsRepository;
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public DeliverySettingsController(

            ISiteService siteService,
            IOrchardServices orchardServices,
            IDeliverySettingsService deliverySettingService,
            IRepository<DeliverySettingRecord> deliverySettingsRepository,
            IRepository<CountryRecord> countryRepository,
            IShapeFactory shapeFactory,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _deliverySettingService = deliverySettingService;
            _deliverySettingsRepository = deliverySettingsRepository;
            _countryRepository = countryRepository;
            Shape = shapeFactory;
            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Index(int? countryId, PagerParameters pagerParameters)
        {

            var viewModel = new DeliverySettingsViewModel
                {
                    CountryId = countryId,
                    CountryRepository = _countryRepository
                };

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            //var setting = _deliverySettingsRepository.Table.Where(s => s.DeliveryCulture == cultureUsed);
            //if (setting.FirstOrDefault() == null)
            //    {
            //        _deliverySettingService.AddSetting("Default", 0, 0, 1, cultureUsed);
            //    }


            var allCountrySettings = _deliverySettingsRepository.Table.Where(s => s.Country.Id == (countryId ?? 0));

            viewModel.DeliverySettings = allCountrySettings
                .OrderBy(a => a.State)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(allCountrySettings.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }


        public ActionResult AddSetting(int countryId)
        {
            var viewModel = new EditDeliverySettingViewModel() { CountryId = countryId };
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult AddSetting(EditDeliverySettingViewModel viewModel)
        {
            _deliverySettingService.
                AddSetting(viewModel.State, viewModel.PostageCost, viewModel.CodCost, viewModel.CountryId,
                    //todo : (auth:juiceek) drop this param
                    cultureUsed);
            return RedirectToAction("Index", new { countryId = viewModel.CountryId});
        }


        public ActionResult DeleteSetting(int id)
        {
            int oldCountryId = _deliverySettingsRepository.Get(id).Country.Id;

            _deliverySettingService.DeleteSetting(id);
            _orchardServices.Notifier.Information(T("Record has been deleted!"));
            return RedirectToAction("Index", new { countryId = oldCountryId });
        }


        public ActionResult EditSetting(int id, int countryId)
        {
            var setting = _deliverySettingService.GetSettingById(id);
            var model = new EditDeliverySettingViewModel()
            {
                Id = setting.Id,
                State = setting.State,
                //DeliveryCost = setting.DeliveryCost,
                CountryId = countryId,
                PostageCost = setting.PostageCost,
                CodCost = setting.CodCost
            };
            return View(model);
        }


        [HttpPost]
        public ActionResult EditSetting(EditDeliverySettingViewModel viewModel)
        {
            _deliverySettingService.EditSetting(viewModel);

            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Index", new { countryId = viewModel.CountryId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Enabled(int id, bool value)
        {
            if (id != null)
            {
                var setting = _deliverySettingService.GetSettingById(id);
                setting.Enabled = value;
                _deliverySettingService.UpdateSetting(setting);
            }
        }



        




    }
}