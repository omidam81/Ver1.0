using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Models;
using Teeyoot.FAQ.Services;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.PaymentSettings.Controllers
{   
    [Admin]
    public class PaymentController : Controller
    {
        private readonly ILanguageService _languageService;
        private readonly IPaymentSettingsService _paymentSettingsService;

        public PaymentController(ILanguageService languageService, IPaymentSettingsService paymentSettingsService)
        {
            _languageService = languageService;
            _paymentSettingsService = paymentSettingsService;
        }

        private const string DEFAULT_LANGUAGE_CODE = "en";

        public ActionResult Index(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                culture = _languageService.GetLanguages().FirstOrDefault(l => l.Code == DEFAULT_LANGUAGE_CODE).Code;
            }

            var languages = _languageService.GetLanguages();

            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(p => p.Culture == culture);
            if (setting == null)
	              return View(new PaymentSettingsViewModel() { Languages = languages, Culture = _languageService.GetLanguageByCode(culture), PaumentMethod = 0 });
		    else
                return View(new PaymentSettingsViewModel() { Languages = languages, Culture = _languageService.GetLanguageByCode(culture), PaumentMethod = setting.PaymentMethod });
	                  
        }




        public ActionResult AddSetting(string language)
        {
            _paymentSettingsService.AddSettings(new PaymentSettingsRecord() { Culture = language, PaymentMethod = 1 });
                return RedirectToAction("Index");
        }

        public ActionResult EditSetting(string language, int paumentMethod)
        {
            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == language);
            setting.PaymentMethod = paumentMethod;
            _paymentSettingsService.UpdateSettings(setting);
            //_paymentSettingsService.AddSettings(new PaymentSettingsRecord() { Culture = language, PaymentMethod = 1 });
            return RedirectToAction("Index", "Payment", new { culture = language });
        }


    }
}