using Orchard;
using Orchard.Localization;
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
        private IOrchardServices Services { get; set; }

        public Localizer T { get; set; }

        public PaymentController(ILanguageService languageService, IPaymentSettingsService paymentSettingsService, IOrchardServices services)
        {
            _languageService = languageService;
            _paymentSettingsService = paymentSettingsService;
            Services = services;
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
                return View(new PaymentSettingsViewModel() { Languages = languages, Culture = _languageService.GetLanguageByCode(culture), CashDeliv = false, CreditCard = false, Mol = false, PayPal = false, SettingEmpty = true });
            else
                return View(new PaymentSettingsViewModel() { Languages = languages, Culture = _languageService.GetLanguageByCode(culture), CashDeliv = setting.CashDeliv, CreditCard = setting.CreditCard, Mol = setting.Mol, PayPal = setting.PayPal, SettingEmpty = false});         
        }

        public ActionResult SaveSettings(bool CashDeliv, bool PayPal, bool Mol, bool CreditCard, string PrivateKey, string PublicKey, string MerchantId,
                                        string ClientToken, string MerchantIdMol, string VerifyKey, string Language)
        {
            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == Language);
            //setting.PaymentMethod = Convert.ToInt32(PaymentMethod);
            setting.PublicKey = PublicKey;
            setting.PrivateKey = PrivateKey;
            setting.MerchantId = MerchantId;
            setting.ClientToken = ClientToken;
            _paymentSettingsService.UpdateSettings(setting);
           return RedirectToAction("Index","Payment", new { culture = "en" });
        }




        public ActionResult AddSetting(string language)
        {
            //_paymentSettingsService.AddSettings(new PaymentSettingsRecord() { Culture = language, PaymentMethod = 1 });
                return RedirectToAction("Index");
        }

        //public ActionResult EditSetting(string language, int paumentMethod)
        //{
        //    var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == language);
        //    setting.PaymentMethod = paumentMethod;
        //    _paymentSettingsService.UpdateSettings(setting);
        //    //_paymentSettingsService.AddSettings(new PaymentSettingsRecord() { Culture = language, PaymentMethod = 1 });
        //    return RedirectToAction("Index", "Payment", new { culture = language });
        //}


    }
}