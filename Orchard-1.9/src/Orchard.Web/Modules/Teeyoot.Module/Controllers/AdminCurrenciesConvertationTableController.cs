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
            IRepository<CurrencyRecord> currencyRepository,
            )
        {
            _siteService = siteService;
            _orchardServices = orchardServices;
            Shape = shapeFactory;
            _currencyRepository = currencyRepository;
        }



        public ActionResult Index()
        {
            return View();
        }

        





    }



}