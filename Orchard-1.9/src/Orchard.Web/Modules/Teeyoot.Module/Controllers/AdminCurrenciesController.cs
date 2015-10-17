using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCurrenciesController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IRepository<LinkCountryCurrencyRecord> _linkCountryCurrencyRepository;

        //todo: (auth:juiceek) drop after applying new logic
        private readonly string _cultureUsed;

        private readonly ImageFileHelper _imageFileHelper;

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminCurrenciesController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IRepository<CurrencyRecord> currencyRepository,
            IRepository<CountryRecord> countryRepository,
            IRepository<LinkCountryCurrencyRecord> linkCountryCurrencyRepository,
            IWorkContextAccessor workContextAccessor)
        {
            var culture = workContextAccessor.GetContext().CurrentCulture.Trim();
            _cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            _currencyRepository = currencyRepository;
            _countryRepository = countryRepository;
            _linkCountryCurrencyRepository = linkCountryCurrencyRepository;

            _imageFileHelper = new ImageFileHelper("currency_{0}_flag.png",
                "/Modules/Teeyoot.Module/Content/images", () => Server);
        }

        public ActionResult Index(PagerParameters pagerParameters)
        {
            var viewModel = new CurrenciesViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var currencies = _linkCountryCurrencyRepository.Table
                .Select(c => new CountryCurrencyItemViewModel
                {
                    Id = c.CurrencyRecord.Id,
                    Code = c.CurrencyRecord.Code,
                    Name = c.CurrencyRecord.Name,
                    ShortName = c.CurrencyRecord.ShortName,
                    CountryId = c.CountryRecord != null ? c.CountryRecord.Id : (int?) null,
                    CountryName = c.CountryRecord != null ? c.CountryRecord.Name : null,
                    FlagFileName = c.CurrencyRecord.FlagFileName
                })
                .OrderBy(c => c.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ToList();

            viewModel.Currencies = currencies;

            var pagerShape = Shape.Pager(pager).TotalItemCount(currencies.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }

        public ActionResult AddCurrency()
        {
            return View(new CurrencyViewModel(_countryRepository));
        }

        [HttpPost]
        public ActionResult AddCurrency(CurrencyViewModel viewModel)
        {
            var currency = new CurrencyRecord
            {
                Code = viewModel.Code,
                Name = viewModel.Name,
                ShortName = viewModel.ShortName,
                CurrencyCulture = _cultureUsed
            };

            bool isNotPng;
            currency.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, currency.Id, out isNotPng);

            if (isNotPng)
            {
                _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                return RedirectToAction("Index");
            }

            var countryCurrency = new LinkCountryCurrencyRecord
            {
                CurrencyRecord = currency
            };

            if (viewModel.CountryId.HasValue)
            {
                var country = _countryRepository.Get(viewModel.CountryId.Value);
                countryCurrency.CountryRecord = country;
            }

            currency.CountryCurrencies.Add(countryCurrency);

            _currencyRepository.Create(currency);

            _orchardServices.Notifier.Information(T("Currency has been added!"));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteCurrency(int id)
        {
            try
            {
                var currency = _currencyRepository.Get(id);
                _currencyRepository.Delete(currency);
            }
            catch (Exception)
            {
                _orchardServices.Notifier.Error(T("Error deleting currency!"));
                return RedirectToAction("Index");
            }

            _imageFileHelper.DeleteImageFromDisc(id);

            _orchardServices.Notifier.Information(T("Record has been deleted!"));
            return RedirectToAction("Index");
        }

        public ActionResult EditCurrency(int id)
        {
            var currency = _currencyRepository.Get(id);

            var viewModel = new CurrencyViewModel(_countryRepository)
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                ShortName = currency.ShortName,
                FlagFileName = currency.FlagFileName
            };

            var country = currency.CountryCurrencies.First().CountryRecord;
            if (country != null)
            {
                viewModel.CountryId = country.Id;
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditCurrency(CurrencyViewModel viewModel)
        {
            var currency = _currencyRepository.Get(viewModel.Id);

            if (viewModel.ImageChanged &&
                !string.IsNullOrEmpty(currency.FlagFileName) &&
                (currency.FlagFileName != viewModel.FlagFileName))
            {
                _imageFileHelper.DeleteImageFromDisc(currency.Id);
            }

            currency.Code = viewModel.Code;
            currency.Name = viewModel.Name;
            currency.ShortName = viewModel.ShortName;
            currency.CurrencyCulture = _cultureUsed;

            if (viewModel.ImageChanged)
            {
                bool isNotPng;
                currency.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, viewModel.Id,
                    out isNotPng);
                if (isNotPng)
                {
                    _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                    return RedirectToAction("EditCurrency", new {id = currency.Id});
                }
            }
            else
            {
                currency.FlagFileName = viewModel.FlagFileName;
            }

            var countryCurrency = currency.CountryCurrencies.First();

            if (viewModel.CountryId.HasValue)
            {
                var country = _countryRepository.Get(viewModel.CountryId.Value);
                countryCurrency.CountryRecord = country;
            }
            else
            {
                countryCurrency.CountryRecord = null;
            }

            _currencyRepository.Update(currency);

            _orchardServices.Notifier.Information(T("Currency has been changed!"));
            return RedirectToAction("Index");
        }
    }
}