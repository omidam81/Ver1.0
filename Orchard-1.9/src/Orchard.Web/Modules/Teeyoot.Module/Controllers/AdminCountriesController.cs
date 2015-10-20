using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Services;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Orchard.Localization.Records;
using Orchard.Logging;

using Teeyoot.Module.Services;



namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCountriesController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IRepository<CultureRecord> _cultureRepository;
        private readonly IRepository<LinkCountryCultureRecord> _linkCountryCultureRepository;
        // test
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ILogger Logger { get; set; }



        public AdminCountriesController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IRepository<CountryRecord> countryRepository,
            IRepository<CultureRecord> cultureRepository,
            IRepository<LinkCountryCultureRecord> linkCountryCultureRepository,
            // test
            ITeeyootMessagingService teeyootMessagingService
            )
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            _countryRepository = countryRepository;
            _cultureRepository = cultureRepository;
            _linkCountryCultureRepository = linkCountryCultureRepository;
            // test
            _teeyootMessagingService = teeyootMessagingService;

            Logger = NullLogger.Instance;
        }



        public ActionResult Index(PagerParameters pagerParameters)
        {
            //Logger.Warning("----------------TETS----------------");
            //Logger.Error("error!!!!!!!!!!!");
            //Logger.Error(new Exception(), "myerror");

            //_teeyootMessagingService.SendExpiredCampaignMessageToSeller(323, true);

            //<<<<<test


            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var allCountries = new List<CountryViewModel>();

            foreach (var record in _countryRepository.Table)
            {
                allCountries.Add(new CountryViewModel()
                {
                    Id = record.Id,
                    Code = record.Code,
                    Name = record.Name,
                    Cultures = GetSelectedCultures(record.Id)
                });
            }

            var viewModel = new CountriesViewModel();
            viewModel.Countries = allCountries
                .OrderBy(a => a.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(viewModel.Countries.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }


        public ActionResult AddCountry()
        {
            return View(new CountryViewModel(GetSelectedCultures(-1)));
        }


        [HttpPost]
        public ActionResult AddCountry(CountryViewModel viewModel)
        {
            bool step1 = false;
            bool step2 = false;

            var record = new CountryRecord()
            {
                Code = viewModel.Code,
                Name = viewModel.Name,
            };
            _countryRepository.Create(record);
            step1 = true;

            if (viewModel.SelectedCultures != null)
            {
                foreach (var cultureId in viewModel.SelectedCultures)
                {
                    var linkCountryCulture = new LinkCountryCultureRecord();
                    linkCountryCulture.CountryRecord = record;
                    linkCountryCulture.CultureRecord = _cultureRepository.Get(cultureId);
                    _linkCountryCultureRepository.Create(linkCountryCulture);
                }
            }
            step2 = true;

            _orchardServices.Notifier.Information(T("Record has been added!"));
            return RedirectToAction("Index");
        }


        public ActionResult DeleteCountry(int id)
        {
            bool step1;
            bool step2;

            foreach(var lnk in _linkCountryCultureRepository.Table.Where(l=>l.CountryRecord.Id == id))
            {
                _linkCountryCultureRepository.Delete(lnk);
            }
            step1 = true;

            var record = _countryRepository.Get(id);
            _countryRepository.Delete(record);
            step2 = true;

            _orchardServices.Notifier.Information(T("Record has been deleted!"));
            return RedirectToAction("Index");
        }


        public ActionResult EditCountry(int id)
        {
            var record = _countryRepository.Get(id);
            var viewModel = new CountryViewModel()
            {
                Id = record.Id,
                Name = record.Name,
                Code = record.Code,
                Cultures = GetSelectedCultures(record.Id)
            };
            return View(viewModel);
        }


        [HttpPost]
        public ActionResult EditCountry(CountryViewModel viewModel)
        {
            bool step1;
            bool step2;
            bool step3;

            var record = _countryRepository.Get(viewModel.Id);
            record.Name = viewModel.Name;
            record.Code = viewModel.Code;
            _countryRepository.Update(record);
            step1 = true;

            foreach (var lnk in _linkCountryCultureRepository.Table.Where(l => l.CountryRecord.Id == viewModel.Id))
            {
                _linkCountryCultureRepository.Delete(lnk);
            }
            step2 = true;

            if (viewModel.SelectedCultures != null)
            {
                foreach (var cultureId in viewModel.SelectedCultures)
                {
                    var linkCountryCulture = new LinkCountryCultureRecord();
                    linkCountryCulture.CountryRecord = record;
                    linkCountryCulture.CultureRecord = _cultureRepository.Get(cultureId);
                    _linkCountryCultureRepository.Create(linkCountryCulture);
                }
            }
            step3 = true;

            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Helper
        /// </summary>
        public IEnumerable<SelectedCultureItem>  GetSelectedCultures(int id)
        {
            var selectedCultures = new List<SelectedCultureItem>();
            foreach (var record in _cultureRepository.Table)
            {
                selectedCultures.Add(new SelectedCultureItem()
                {
                    Id = record.Id,
                    Culture = record.Culture,
                    Selected = _linkCountryCultureRepository.Table.Any( x=> (x.CountryRecord.Id == id)&&(x.CultureRecord.Id == record.Id) )
                });
            }
            return selectedCultures;

        }


    }



}