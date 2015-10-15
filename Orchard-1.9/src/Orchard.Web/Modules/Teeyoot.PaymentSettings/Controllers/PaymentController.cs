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
        private readonly IPaymentSettingsService _paymentSettingsService;
        private IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        public Localizer T { get; set; }

        public PaymentController(IWorkContextAccessor workContextAccessor, IPaymentSettingsService paymentSettingsService, IOrchardServices services)
        {
            _workContextAccessor = workContextAccessor;
            _paymentSettingsService = paymentSettingsService;
            Services = services;

            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public ActionResult Index(string culture)
        {
            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault();
            if (setting == null)
                return View(new PaymentSettingsViewModel() { CashDeliv = false, CreditCard = false, Mol = false, PayPal = false, SettingEmpty = true });
            else
                return View(new PaymentSettingsViewModel() { merchantId = setting.MerchantId, clientToken = setting.ClientToken, merchantIdMol = setting.MerchantIdMol, privateKey = setting.PrivateKey, verifyKey = setting.VerifyKey, publicKey = setting.PublicKey, CashDeliv = setting.CashDeliv, CreditCard = setting.CreditCard, Mol = setting.Mol, PayPal = setting.PayPal, SettingEmpty = false,
                    // Tab names for payment methods
                    CashDelivTabName = setting.CashDelivTabName,
                    PayPalTabName = setting.PayPalTabName,
                    MolTabName = setting.MolTabName,
                    CreditCardTabName = setting.CreditCardTabName,
                    // Notes for payment methods
                    CashDelivNote = setting.CashDelivNote,
                    PayPalNote = setting.PayPalNote,
                    MolNote = setting.MolNote,
                    CreditCardNote = setting.CreditCardNote}); 
        }

        public ActionResult SaveSettings(bool CashDeliv, bool PayPal, bool Mol, bool CreditCard, string PrivateKey, string PublicKey, string MerchantId,
                                        string ClientToken, string MerchantIdMol, string VerifyKey,
                                        // Tab names for payment methods
                                        string CashDelivTabName,
                                        string PayPalTabName,
                                        string MolTabName,
                                        string CreditCardTabName,
                                        // Notes for payment methods
                                        string CashDelivNote,
                                        string PayPalNote,
                                        string MolNote,
                                        string CreditCardNote)
        {
            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault();
            //setting.PaymentMethod = Convert.ToInt32(PaymentMethod);
            setting.PublicKey = PublicKey;
            setting.PrivateKey = PrivateKey;
            setting.MerchantId = MerchantId;
            setting.MerchantIdMol = MerchantIdMol;
            setting.VerifyKey = VerifyKey;
            setting.ClientToken = ClientToken;
            setting.CashDeliv = CashDeliv;
            setting.PayPal = PayPal;
            setting.Mol = Mol;
            setting.CreditCard = CreditCard;

            // Tab names for payment methods
            setting.CashDelivTabName = CashDelivTabName;
            setting.PayPalTabName = PayPalTabName;
            setting.MolTabName = MolTabName;
            setting.CreditCardTabName = CreditCardTabName;
            // Notes for payment methods
            setting.CashDelivNote = CashDelivNote;
            setting.PayPalNote = PayPalNote;
            setting.MolNote = MolNote;
            setting.CreditCardNote = CreditCardNote;

            _paymentSettingsService.UpdateSettings(setting);
            return RedirectToAction("Index", "Payment");
        }

        public ActionResult AddSetting()
        {
            _paymentSettingsService.AddSettings(new PaymentSettingsRecord() { Culture = cultureUsed });
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