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

        private readonly ImageFileHelper _imageFileHelper; 
        
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminCurrenciesController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IRepository<CurrencyRecord> currencyRepository,
            IRepository<CountryRecord> countryRepository,
            IRepository<LinkCountryCurrencyRecord> linkCountryCurrencyRepository
            )
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            _currencyRepository = currencyRepository;
            _countryRepository = countryRepository;
            _linkCountryCurrencyRepository = linkCountryCurrencyRepository;

            _imageFileHelper = new ImageFileHelper("currency_{0}_flag.png", 
                                    "/Modules/Teeyoot.Module/Content/images", () => this.Server);
        }



        public ActionResult ConvertationTable()
        {
            return View();
        }

        public ActionResult Currencies(PagerParameters pagerParameters)
        {
            var viewModel = new CurrenciesViewModel();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var allCurrencies = new List<CurrencyViewModel>(); 
            foreach(var record in _currencyRepository.Table)
            {
                allCurrencies.Add(new CurrencyViewModel(_countryRepository){
                    Id = record.Id,
                    Code = record.Code,
                    Name = record.Name,
                    ShortName = record.ShortName,
                    CountryId = GetCountryByCurrency(record.Id),
                    CountryName = GetCountryName( GetCountryByCurrency(record.Id) ),
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
            bool step1_CurrencySaved = false;
            bool step2_ImageFileSaved = false;
            bool step3_CountryForCurrencySaved = false;
            bool step4_CurrencyResaved = false;
            try
            {
                var record = new CurrencyRecord
                {
                    Id = viewModel.Id,
                    Code = viewModel.Code,
                    Name = viewModel.Name,
                    ShortName = viewModel.ShortName,
                    CurrencyCulture = "FAKE_CULT"
                };
                _currencyRepository.Create(record);
                step1_CurrencySaved = true;
            
                bool isNotPNG;
                record.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, record.Id, out isNotPNG);
                if (isNotPNG)
                {
                    _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                    return RedirectToAction("Currencies");
                }
                step2_ImageFileSaved = true;

                SetCountryForCurrency(record.Id, viewModel.CountryId);
                step3_CountryForCurrencySaved = true;

                _currencyRepository.Update(record);
                step4_CurrencyResaved = true;
            }
            catch(Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }
                        
            return RedirectToAction("Currencies");
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
            catch(Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }

            _orchardServices.Notifier.Information(T("Currency has been deleted!"));
            return RedirectToAction("Currencies");
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
            var oldrecord = _currencyRepository.Get(viewModel.Id);
            if (viewModel.ImageChanged && 
                !String.IsNullOrEmpty(oldrecord.FlagFileName) &&
                   (oldrecord.FlagFileName != viewModel.FlagFileName))
            {
                _imageFileHelper.DeleteImageFromDisc(oldrecord.Id);
            }
            // Updating values by new ones.
            var record = new CurrencyRecord
            {
                Id = viewModel.Id,
                Code = viewModel.Code,
                Name = viewModel.Name,
                ShortName = viewModel.ShortName,
                CurrencyCulture = "FAKE_CULT"
            };
            // Saving in transaction.
            bool step1_ImageFileSaved = false;
            bool step2_CountryForCurrencySaved = false;
            bool step3_CurrencySaved = false;
            try
            {
                if (viewModel.ImageChanged)
                {
                    bool isNotPNG;
                    record.FlagFileName = _imageFileHelper.SaveImageToDisc(viewModel.FlagImage, viewModel.Id, out isNotPNG);
                    if (isNotPNG)
                    {
                        _orchardServices.Notifier.Error(T("Flag Image file must be *.png."));
                        return RedirectToAction("Currencies");
                    }
                }
                else
                {
                    record.FlagFileName = viewModel.FlagFileName;
                }

                step1_ImageFileSaved = true;

                SetCountryForCurrency(viewModel.Id, viewModel.CountryId);
                step2_CountryForCurrencySaved = true;

                _currencyRepository.Update(record);
                step3_CurrencySaved = true;
            }
            catch (Exception)
            {
                //todo: (auth:Juiceek) Add rollback transaction logic
                throw;
            }

            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Currencies");
        }




        #region ################# INNER_HELPERS >>> #################################################

        /// <summary>
        /// Helper function
        /// </summary>
        private void ClearAllCurrencyToCountryLinks(int currencyId)
        {
            var query = _linkCountryCurrencyRepository.Table.Where(x => x.CurrencyRecord.Id == currencyId);
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
            // Claar the old links.
            ClearAllCurrencyToCountryLinks(currencyId);
            // Relink. 
            if (countryId != null)
            {
                var newLnk = new LinkCountryCurrencyRecord() {
                                    CurrencyRecord = _currencyRepository.Get(currencyId),
                                    CountryRecord = _countryRepository.Get((int)countryId)
                                };
                _linkCountryCurrencyRepository.Create(newLnk);
            }
        }

        /// <summary>
        /// Helper function 
        /// </summary>
        private int? GetCountryByCurrency(int currencyId)
        {
            var result = _linkCountryCurrencyRepository.Table.Where(x => x.CurrencyRecord.Id == currencyId).Select( x => x.CountryRecord.Id).FirstOrDefault();
            if (result != 0) {
                return result;
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Helper function 
        /// </summary>
        private string GetCountryName(int? countryId)
        {
            if (countryId == null) 
            {
                return null;
            }
            return _countryRepository.Get((int)countryId).Name;
        }

        #endregion ######################################### <<< INNER HELPERS ######################

    }



}