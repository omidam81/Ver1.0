using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Services;
using Orchard.Localization;
using Orchard.Logging;
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
    public class AdminCurrenciesConvertationTableController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;

        private readonly IRepository<CurrencyExchangeRecord> _currencyExchangeRepository;
        private readonly IRepository<CurrencyRecord> _currencyRepository;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        private readonly IWorkContextAccessor _workContextAccessor;


        public AdminCurrenciesConvertationTableController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IRepository<CurrencyExchangeRecord> currencyExchangeRepository,
            IRepository<CurrencyRecord> currencyRepository,
            IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            _currencyExchangeRepository = currencyExchangeRepository;
            _currencyRepository = currencyRepository;

            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }



        public ActionResult Index()
        {
            var exchangeMatrix = new CurrencyExchangeMatrixViewModel();
            exchangeMatrix.Columns = new List<List<CurrencyExchangeViewModel>>();

            int x = 0;
            foreach (var currencyTo in _currencyRepository.Table)
            {
                int y = 0;
                var newColumn = new List<CurrencyExchangeViewModel>();
                foreach (var currencyFrom in _currencyRepository.Table)
                {
                    var exchange = new CurrencyExchangeViewModel();
                    // If it won't be a delivery from a country to the same country
                    // then populate the matrix with the real data.
                    if (x != y)
                    {
                        var record = _currencyExchangeRepository.Table
                            .Where(s => (s.CurrencyFrom.Id == currencyFrom.Id) && (s.CurrencyTo.Id == currencyTo.Id))
                            .FirstOrDefault();
                        if (record == null)
                        {
                            record = new CurrencyExchangeRecord();
                            record.CurrencyFrom = currencyFrom;
                            record.CurrencyTo = currencyTo;
                            _currencyExchangeRepository.Create(record);
                        }
                        exchange.Id = record.Id;
                        exchange.CurrencyFromId = record.CurrencyFrom.Id;
                        exchange.CurrencyFromCode = record.CurrencyFrom.Code;
                        exchange.CurrencyToId = record.CurrencyTo.Id;
                        exchange.CurrencyToCode = record.CurrencyTo.Code;
                        exchange.RateForBuyer = record.RateForBuyer;
                        exchange.RateForSeller = record.RateForSeller;
                        exchange.CurrencyFromFlagFileName = record.CurrencyFrom.FlagFileName;
                        exchange.CurrencyToFlagFileName = record.CurrencyTo.FlagFileName;

                    }
                    // It's an exchange from a currency to the same currency.
                    // just pass the empty setting to the view.
                    else
                    {
                        exchange.CurrencyFromCode = currencyFrom.Code;
                        exchange.CurrencyToCode = currencyTo.Code;
                        exchange.CurrencyFromFlagFileName = currencyFrom.FlagFileName;
                        exchange.CurrencyToFlagFileName = currencyTo.FlagFileName;
                    }
                    newColumn.Add(exchange);
                    y++;
                }
                exchangeMatrix.Columns.Add(newColumn);
                x++;
            }

            return View(exchangeMatrix);
        }


        [HttpPost]
        public ActionResult Edit(CurrencyExchangeMatrixViewModel viewModel)
        {
            int x = 0;
            foreach (var column in viewModel.Columns)
            {
                int y = 0;
                foreach (var newExchange in column)
                {
                    // Dont save exchange rates from a currency to the same currency
                    if (x != y)
                    {
                        var record = _currencyExchangeRepository.Table
                                .Where(s => (s.CurrencyFrom.Id == newExchange.CurrencyFromId)
                                    && (s.CurrencyTo.Id == newExchange.CurrencyToId))
                                .First();

                        record.CurrencyFrom = _currencyRepository.Get(newExchange.CurrencyFromId);
                        record.CurrencyTo = _currencyRepository.Get(newExchange.CurrencyToId);
                        record.RateForBuyer = newExchange.RateForBuyer;
                        record.RateForSeller = newExchange.RateForSeller;
                        _currencyExchangeRepository.Update(record);
                    }
                    y++;
                }
                x++;
            }

            _orchardServices.Notifier.Information(T("Settings have been changed!"));
            return RedirectToAction("Index");
        }



    }

    #region ###########################3 LEGACY_STUFF (DELETE WHEN DONE) >>>>>> ########################33


    //[Admin]
    //public class AdminCurrenciesConvertationTableController : Controller
    //{
    //    private readonly ISiteService _siteService;
    //    private readonly IOrchardServices _orchardServices;
    //    private readonly IRepository<CurrencyRecord> _currencyRepository;

    //    private dynamic Shape { get; set; }
    //    public Localizer T { get; set; }

    //    public AdminCurrenciesConvertationTableController(
    //        ISiteService siteService,
    //        IOrchardServices orchardServices,
    //        IShapeFactory shapeFactory,
    //        IRepository<CurrencyRecord> currencyRepository
    //        )
    //    {
    //        _siteService = siteService;
    //        _orchardServices = orchardServices;
    //        Shape = shapeFactory;
    //        _currencyRepository = currencyRepository;
    //    }



    //    public ActionResult Index(PagerParameters pagerParameters)
    //    {
    //        var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

    //        var allCurrencies = new List<CurrencyViewModel>();
    //        foreach (var record in _currencyRepository.Table)
    //        {
    //            allCurrencies.Add(new CurrencyViewModel()
    //            {
    //                Id = record.Id,
    //                Code = record.Code,
    //                Name = record.Name,
    //                PriceBuyers = record.PriceBuyers,
    //                PriceSellers = record.PriceSellers,
    //                IsConvert = record.IsConvert
    //            });
    //        }

    //        var viewModel = new CurrenciesViewModel();
    //        viewModel.Currencies = allCurrencies
    //            .Where(x => x.IsConvert == true)
    //            .OrderBy(a => a.Name)
    //            .Skip(pager.GetStartIndex())
    //            .Take(pager.PageSize);

    //        var pagerShape = Shape.Pager(pager).TotalItemCount(viewModel.Currencies.Count());
    //        viewModel.Pager = pagerShape;

    //        return View(viewModel);
    //    }


    //    public ActionResult AddConvertation()
    //    {
    //        return View(new CurrencyViewModel(_currencyRepository) { PriceBuyers = 1, PriceSellers = 1 });
    //    }


    //    [HttpPost]
    //    public ActionResult AddConvertation(CurrencyViewModel viewModel)
    //    {
    //        var record = _currencyRepository.Get(viewModel.Id);
    //        record.PriceBuyers = viewModel.PriceBuyers;
    //        record.PriceSellers = viewModel.PriceSellers;
    //        record.IsConvert = true;
    //        _currencyRepository.Update(record);

    //        _orchardServices.Notifier.Information(T("Record has been added!"));
    //        return RedirectToAction("Index");
    //    }


    //    public ActionResult DeleteConvertation(int id)
    //    {
    //        var record = _currencyRepository.Get(id);
    //        record.PriceBuyers = 1;
    //        record.PriceSellers = 1;
    //        record.IsConvert = false;
    //        _currencyRepository.Update(record);

    //        _orchardServices.Notifier.Information(T("Record has been deleted!"));
    //        return RedirectToAction("Index");
    //    }


    //    public ActionResult EditConvertation(int id)
    //    {
    //        var record = _currencyRepository.Get(id);
    //        var viewModel = new CurrencyViewModel(_currencyRepository)
    //        {
    //            Id = record.Id,
    //            Name = record.Name,
    //            PriceBuyers = record.PriceBuyers,
    //            PriceSellers = record.PriceSellers,
    //            IsConvert = record.IsConvert
    //        };
    //        return View(viewModel);
    //    }

    //    [HttpPost]
    //    public ActionResult EditConvertation(CurrencyViewModel viewModel)
    //    {
    //        var record = _currencyRepository.Get(viewModel.Id);
    //        record.PriceBuyers = viewModel.PriceBuyers;
    //        record.PriceSellers = viewModel.PriceSellers;
    //        _currencyRepository.Update(record);

    //        _orchardServices.Notifier.Information(T("Record has been changed!"));
    //        return RedirectToAction("Index");
    //    }






    //}

    #endregion ######################## <<< LEGACY_STUFF ###################################################

}