using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
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
    public class DeliveryInternationalSettingsController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        
        private readonly IRepository<DeliveryInternationalSettingRecord> _deliveryInternationalSettingRepository;
        private readonly IRepository<CountryRecord> _countryRepository;
        
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;


        public DeliveryInternationalSettingsController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<DeliveryInternationalSettingRecord> deliveryInternationalSettingRepository,
            IRepository<CountryRecord> countryRepository,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _deliveryInternationalSettingRepository = deliveryInternationalSettingRepository;
            _countryRepository = countryRepository;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }



        public ActionResult Index()
        {
            var deliveryMatrix = new DeliveryMatrixViewModel();
            deliveryMatrix.Columns = new List<List<DeliveryInternationalSettingViewModel>>();

            int x = 0;
            foreach (var countryTo in _countryRepository.Table)
            {
                int y = 0;
                var newColumn = new List<DeliveryInternationalSettingViewModel>();
                foreach (var countryFrom in _countryRepository.Table)
                {
                    var setting = new DeliveryInternationalSettingViewModel();
                    // If it won't be a delivery from a country to the same country
                    // then populate the matrix with the real data.
                    if (x != y)
                    {
                        var record = _deliveryInternationalSettingRepository.Table
                            .Where(s => (s.CountryFrom.Id == countryFrom.Id) && (s.CountryTo.Id == countryTo.Id))
                            .FirstOrDefault();
                        if (record == null)
                        {
                            record = new DeliveryInternationalSettingRecord();
                            record.CountryFrom = countryFrom;
                            record.CountryTo = countryTo;
                            record.IsActive = false;
                            _deliveryInternationalSettingRepository.Create(record);
                        }
                        setting.Id = record.Id;
                        setting.CountryFromId = record.CountryFrom.Id;
                        setting.CountryFromName = record.CountryFrom.Name;
                        setting.CountryToId = record.CountryTo.Id;
                        setting.CountryToName = record.CountryTo.Name;
                        setting.DeliveryPrice = record.DeliveryPrice;
                        setting.IsActive = record.IsActive;
                    }
                    // It's a delivery from a country to the same country.
                    // just pass the empty setting to the view.
                    else
                    {
                        setting.CountryFromName = countryFrom.Name;
                        setting.CountryToName = countryTo.Name;
                    }
                    newColumn.Add(setting);
                    y++;
                }
                deliveryMatrix.Columns.Add(newColumn);
                x++;
            }

            return View(deliveryMatrix);
        }


        [HttpPost]
        public ActionResult Edit(DeliveryMatrixViewModel viewModel)
        {
            int x = 0;
            foreach(var column in viewModel.Columns)
            {
                int y = 0;
                foreach(var newSetting in column)
                {
                    // Dont save delivery settings from a country to the same country
                    if (x != y)
                    {
                        var record = _deliveryInternationalSettingRepository.Table
                                .Where(s => (s.CountryFrom.Id == newSetting.CountryFromId)
                                    && (s.CountryTo.Id == newSetting.CountryToId))
                                .First();

                        record.CountryFrom = _countryRepository.Get(newSetting.CountryFromId);
                        record.CountryTo = _countryRepository.Get(newSetting.CountryToId);
                        record.DeliveryPrice = newSetting.DeliveryPrice;
                        record.IsActive = newSetting.IsActive;
                        _deliveryInternationalSettingRepository.Update(record);
                    }
                    y++;
                }
                x++;
            }

            _orchardServices.Notifier.Information(T("Settings have been changed!"));
            return RedirectToAction("Index");
        }



    }



}