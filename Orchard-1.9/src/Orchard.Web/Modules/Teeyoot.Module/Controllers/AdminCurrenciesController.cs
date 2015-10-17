using System;
using System.Collections.Generic;
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
        private readonly IWorkContextAccessor _workContextAccessor;
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

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
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

            var allCurrencies = new List<CurrencyViewModel>();
            foreach (var record in _currencyRepository.Table)
            {
                allCurrencies.Add(new CurrencyViewModel(_countryRepository)
                {
                    Id = record.Id,
                    Code = record.Code,
                    Name = record.Name,
                    ShortName = record.ShortName,
                    CountryId = GetCountryByCurrency(record.Id),
                    CountryName = GetCountryName(GetCountryByCurrency(record.Id)),
                    FlagFileName = record.FlagFileName
                });
            }

            viewModel.Currencies = allCurrencies
                .OrderBy(a => a.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(allCurrencies.Count());
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
            // Saving in transaction.
            var step1CurrencySaved = false;
            var step2ImageFileSaved = false;
            var step3CountryForCurrencySaved = false;
            var step4CurrencyResaved = false;
            try
            {
                var currency = new CurrencyRecord
                {
                    Code = viewModel.Code,
                    Name = viewModel.Name,
                    ShortName = viewModel.ShortName,
                    CurrencyCulture = _cultureUsed
                };
                _currencyRepository.Create(currency);
                step1CurrencySaved = true;

                bool isNotPng;
                currency.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, currency.Id, out isNotPng);
                if (isNotPng)
                {
                    _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                    return RedirectToAction("Currencies");
                }
                step2ImageFileSaved = true;

                SetCountryForCurrency(currency.Id, viewModel.CountryId);
                step3CountryForCurrencySaved = true;

                _currencyRepository.Update(currency);
                step4CurrencyResaved = true;
            }
            catch (Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }

            _orchardServices.Notifier.Information(T("Record has been added!"));
            return RedirectToAction("Index");
        }

        public ActionResult DeleteCurrency(int id)
        {
            // Deleting in transaction.
            try
            {
                ClearAllCurrencyToCountryLinks(id);
                _currencyRepository.Delete(_currencyRepository.Get(id));
                _imageFileHelper.DeleteImageFromDisc(id);
            }
            catch (Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }

            _orchardServices.Notifier.Information(T("Record has been deleted!"));
            return RedirectToAction("Index");
        }

        public ActionResult EditCurrency(int id)
        {
            var record = _currencyRepository.Get(id);
            var viewModel = new CurrencyViewModel(_countryRepository)
            {
                Id = record.Id,
                Code = record.Code,
                Name = record.Name,
                ShortName = record.ShortName,
                CountryId = GetCountryByCurrency(record.Id),
                FlagFileName = record.FlagFileName
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditCurrency(CurrencyViewModel viewModel)
        {
            // Getting old record to determine if the image is changed,
            // than we must clear out the old image.
            var currency = _currencyRepository.Get(viewModel.Id);

            if (viewModel.ImageChanged &&
                !string.IsNullOrEmpty(currency.FlagFileName) &&
                (currency.FlagFileName != viewModel.FlagFileName))
            {
                _imageFileHelper.DeleteImageFromDisc(currency.Id);
            }

            // Updating values by new ones.
            currency.Code = viewModel.Code;
            currency.Name = viewModel.Name;
            currency.ShortName = viewModel.ShortName;
            currency.CurrencyCulture = _cultureUsed;

            // Saving in transaction.
            var step1ImageFileSaved = false;
            var step2CountryForCurrencySaved = false;
            var step3CurrencySaved = false;
            try
            {
                if (viewModel.ImageChanged)
                {
                    bool isNotPng;
                    currency.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, viewModel.Id,
                        out isNotPng);
                    if (isNotPng)
                    {
                        _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                        return RedirectToAction("Currencies");
                    }
                }
                else
                {
                    currency.FlagFileName = viewModel.FlagFileName;
                }
                step1ImageFileSaved = true;

                SetCountryForCurrency(viewModel.Id, viewModel.CountryId);
                step2CountryForCurrencySaved = true;

                _currencyRepository.Update(currency);
                step3CurrencySaved = true;
            }
            catch (Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }

            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Helper function
        /// </summary>
        private void ClearAllCurrencyToCountryLinks(int currencyId)
        {
            var query = _linkCountryCurrencyRepository.Table
                .Where(x => x.CurrencyRecord.Id == currencyId);

            foreach (var lnk in query)
            {
                _linkCountryCurrencyRepository.Delete(lnk);
            }
        }

        /// <summary>
        /// Helper function
        /// </summary>
        private void SetCountryForCurrency(int currencyId, int? countryId)
        {
            // Clear the old links.
            ClearAllCurrencyToCountryLinks(currencyId);
            if (countryId == null)
            {
                return;
            }

            // Relink. 
            var newLnk = new LinkCountryCurrencyRecord
            {
                CurrencyRecord = _currencyRepository.Get(currencyId),
                CountryRecord = _countryRepository.Get((int) countryId)
            };

            _linkCountryCurrencyRepository.Create(newLnk);
        }

        /// <summary>
        /// Helper function 
        /// </summary>
        private int? GetCountryByCurrency(int currencyId)
        {
            var result = _linkCountryCurrencyRepository.Table
                .Where(x => x.CurrencyRecord.Id == currencyId)
                .Select(x => x.CountryRecord.Id)
                .FirstOrDefault();

            if (result != 0)
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Helper function 
        /// </summary>
        private string GetCountryName(int? countryId)
        {
            return countryId == null ? null : _countryRepository.Get((int) countryId).Name;
        }
    }
}