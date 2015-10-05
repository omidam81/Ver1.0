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
    public class AdminCurrenciesConvertationTableController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<CurrencyRecord> _currencyRepository;

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminCurrenciesConvertationTableController(
            ISiteService siteService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            IRepository<CurrencyRecord> currencyRepository
            )
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            _currencyRepository = currencyRepository;
        }



        public ActionResult Index(PagerParameters pagerParameters)
        {
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            var allCurrencies = new List<CurrencyViewModel>();
            foreach (var record in _currencyRepository.Table)
            {
                allCurrencies.Add(new CurrencyViewModel()
                {
                    Id = record.Id,
                    Code = record.Code,
                    Name = record.Name,
                    PriceBuyers = record.PriceBuyers,
                    PriceSellers = record.PriceSellers,
                    IsConvert = record.IsConvert
                });
            }

            var viewModel = new CurrenciesViewModel();
            viewModel.Currencies = allCurrencies
                .Where(x => x.IsConvert == true)
                .OrderBy(a => a.Name)
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize);

            var pagerShape = Shape.Pager(pager).TotalItemCount(viewModel.Currencies.Count());
            viewModel.Pager = pagerShape;

            return View(viewModel);
        }


        public ActionResult AddConvertation()
        {
            return View(new CurrencyViewModel(_currencyRepository) { PriceBuyers = 1, PriceSellers = 1 });
        }


        [HttpPost]
        public ActionResult AddConvertation(CurrencyViewModel viewModel)
        {
            var record = _currencyRepository.Get(viewModel.Id);
            record.PriceBuyers = viewModel.PriceBuyers;
            record.PriceSellers = viewModel.PriceSellers;
            record.IsConvert = true;
            _currencyRepository.Update(record);

            _orchardServices.Notifier.Information(T("Record has been added!"));
            return RedirectToAction("Index");
        }


        public ActionResult DeleteConvertation(int id)
        {
            var record = _currencyRepository.Get(id);
            record.PriceBuyers = 1;
            record.PriceSellers = 1;
            record.IsConvert = false;
            _currencyRepository.Update(record);

            _orchardServices.Notifier.Information(T("Record has been added!"));
            return RedirectToAction("Index");
        }


        public ActionResult EditConvertation(int id)
        {
            var record = _currencyRepository.Get(id);
            var viewModel = new CurrencyViewModel(_currencyRepository)
            {
                Id = record.Id,
                Name = record.Name,
                PriceBuyers = record.PriceBuyers,
                PriceSellers = record.PriceSellers,
                IsConvert = record.IsConvert
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditConvertation(CurrencyViewModel viewModel)
        {
            var record = _currencyRepository.Get(viewModel.Id);
            record.PriceBuyers = viewModel.PriceBuyers;
            record.PriceSellers = viewModel.PriceSellers;
            _currencyRepository.Update(record);

            _orchardServices.Notifier.Information(T("Record has been changed!"));
            return RedirectToAction("Index");
        }
        





    }



}