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

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public DeliverySettingsController(

            ISiteService siteService,
            IOrchardServices orchardServices,
            IDeliverySettingsService deliverySettingService,
            IRepository<DeliverySettingRecord> deliverySettingsRepository,
            IShapeFactory shapeFactory)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _deliverySettingService = deliverySettingService;
            _deliverySettingsRepository = deliverySettingsRepository;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var viewModel = new DeliverySettingsViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            viewModel.DeliverySettings = _deliverySettingsRepository.Table
                .OrderBy(a => a.State)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(_deliverySettingsRepository.Table.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }

        public ActionResult AddSetting()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSetting(DeliverySettingRecord record)
        {
            _deliverySettingService.AddSetting(record.State, record.DeliveryCost);
            return RedirectToAction("Index");
        }

        public ActionResult DeleteSetting(int id)
        {
            _deliverySettingService.DeleteSetting(id);
            _orchardServices.Notifier.Information(T("State has been deleted!"));
            return RedirectToAction("Index");
        }

        public ActionResult EditSetting(int id)
        {
            var setting = _deliverySettingService.GetSettingById(id);
            var model = new EditDeliverySettingViewModel()
            {
                Id = setting.Id,
                State = setting.State,
                DeliveryCost = setting.DeliveryCost

            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSetting(EditDeliverySettingViewModel model)
        {
            _deliverySettingService.EditSetting(model);
            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Index");
        }

        public JsonResult GetSettings()
        {
            var settings = _deliverySettingService.GetAllSettings().ToArray();
            return Json(new
            {
                settings  = settings
            }, JsonRequestBehavior.AllowGet);
        }




    }
}