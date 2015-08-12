using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Services;

namespace Teeyoot.PaymentSettings.Controllers
{   
    [Admin]
    public class PaymentController : Controller
    {
        private readonly ILanguageService _languageService;

        public PaymentController( ILanguageService languageService)
        {
            _languageService = languageService;
        }


        public ActionResult Index()
        {
            return View();
        }
    }
}