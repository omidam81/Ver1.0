using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Messaging.ViewModels;
using Mandrill;
using Mandrill.Model;
using Teeyoot.FAQ.Services;
using System.Collections.Specialized;


namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminMessageController : Controller, IUpdateModel
    {
        private readonly IMailChimpSettingsService _settingsService;
        private readonly ILanguageService _languageService;

        public AdminMessageController(IMailChimpSettingsService settingsService, IOrchardServices services, ILanguageService languageService)
        {
            _settingsService = settingsService;
            Services = services;
            _languageService = languageService;
        }

        private IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        private const string DEFAULT_LANGUAGE_CODE = "en";

        public ActionResult Index(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                    culture = _languageService.GetLanguages().FirstOrDefault(l => l.Code == DEFAULT_LANGUAGE_CODE).Code;
            }
            var availableLanguages = _languageService.GetLanguages().ToList();
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var settings = _settingsService.GetSettingByCulture(culture).List().Select(s => new MailChimpListViewModel
            {
                Id = s.Id,
                ApiKey = s.ApiKey,
                SellerTemplate = System.IO.File.Exists(pathToTemplates + "seller-template.html") ? "seller-template.html" : "No file!",
                WelcomeTemplate = System.IO.File.Exists(pathToTemplates + "welcome-template.html") ? "welcome-template.html" : "No file!",
                RelaunchTemplate = System.IO.File.Exists(pathToTemplates + "relaunch-template.html") ? "relaunch-template.html" : "No file!",
                LaunchTemplate = System.IO.File.Exists(pathToTemplates + "launch-template.html") ? "launch-template.html" : "No file!",
                WithdrawTemplate = System.IO.File.Exists(pathToTemplates + "withdraw-template.html") ? "withdraw-template.html" : "No file!",
                PlaceOrderTemplate = System.IO.File.Exists(pathToTemplates + "place-order-template.html") ? "place-order-template.html" : "No file!",
                NewOrderTemplate = System.IO.File.Exists(pathToTemplates + "new-order-template.html") ? "new-order-template.html" : "No file!",
                ShippedOrderTemplate = System.IO.File.Exists(pathToTemplates + "shipped-order-template.html") ? "shipped-order-template.html" : "No file!",
                DeliveredOrderTemplate = System.IO.File.Exists(pathToTemplates + "delivered-order-template.html") ? "delivered-order-template.html" : "No file!",
                CancelledOrderTemplate = System.IO.File.Exists(pathToTemplates + "cancelled-order-template.html") ? "cancelled-order-template.html" : "No file!",
                OrderIsPrintingBuyerTemplate = System.IO.File.Exists(pathToTemplates + "order-is-printing-buyer-template.html") ? "order-is-printing-buyer-template.html" : "No file!",
                CampaignIsPrintingSellerTemplate = System.IO.File.Exists(pathToTemplates + "campaign-is-printing-seller-template.html") ? "campaign-is-printing-seller-template.html" : "No file!",
                PaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "paid-campaign-template.html") ? "paid-campaign-template.html" : "No file!",
                UnpaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "unpaid-campaign-template.html") ? "unpaid-campaign-template.html" : "No file!",
                CampaignNotReachGoalBuyerTemplate = System.IO.File.Exists(pathToTemplates + "not-reach-goal-seller-template.html") ? "not-reach-goal-seller-template.html" : "No file!",
                CampaignNotReachGoalSellerTemplate = System.IO.File.Exists(pathToTemplates + "not-reach-goal-buyer-template.html") ? "not-reach-goal-buyer-template.html" : "No file!",
                PartiallyPaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "partially-paid-campaign-template.html") ? "partially-paid-campaign-template.html" : "No file!",
                CampaignPromoTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                AllOrderDeliveredTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                CampaignIsFinishedTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                DefinitelyGoSellerTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                DefinitelyGoBuyerTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                EditedCampaignTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                ExpiredMetMinimumTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                ExpiredNotSuccessfullTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                ExpiredSuccessfullTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                MakeTheOrderTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                NewCampaignAdminTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                NewOrderBuyerTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                NotReachGoalMetMinimumTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                RecoverOrdersTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                RejectTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                Shipped3DayAfterTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                TermsConditionsTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                WithdrawCompletedTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template.html") ? "campaign-promo-template.html" : "No file!",
                Culture = s.Culture,
                AvailableLanguages = availableLanguages,

            });
            if (settings.FirstOrDefault() == null)
            {
                var settingPart = new MailChimpListViewModel() {
                    Culture = culture,
                    AvailableLanguages = availableLanguages,               
                };
                return View(settingPart);
            }
                     
            return View(settings.FirstOrDefault());
        }

        public ActionResult AddSetting(string returnUrl, string language)
        {
            MailChimpSettingsPart mailChimpSettingsPart = Services.ContentManager.New<MailChimpSettingsPart>("MailChimpSettings");

            if (mailChimpSettingsPart == null)
                return HttpNotFound();
            try
            {
                var culture = _languageService.GetLanguages().ToList().Where(l=>l.Code == language);
                mailChimpSettingsPart.AvailableLanguages = culture;
                var model = Services.ContentManager.BuildEditor(mailChimpSettingsPart);
                return View(model);
            }
            catch (Exception exception)
            {
                Logger.Error(T("Creating setting failed: {0}", exception.Message).Text);
                Services.Notifier.Error(T("Creating setting failed: {0}", exception.Message));
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            }
        }

        [HttpPost, ActionName("AddSetting")]
        public ActionResult AddSettingPOST(string returnUrl)
        {
            Uri myUri = new Uri(returnUrl);
            var culture = HttpUtility.ParseQueryString(myUri.Query).Get("Culture");
            var mailChimpSettingPart = _settingsService.CreateMailChimpSettingsPart("",culture);
            if (mailChimpSettingPart == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(mailChimpSettingPart, this);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    Services.Notifier.Error(T(error));
                }
                return View(model);
            }

            Services.Notifier.Information(T("New setting has been added."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }


        public ActionResult EditMailChimpSetting(int id)
        {
            var mailChimpSettingPart = _settingsService.GetSetting(id);
            var language = _languageService.GetLanguages().ToList().Where(l => l.Code == mailChimpSettingPart.Culture);
            mailChimpSettingPart.AvailableLanguages = language;
            if (mailChimpSettingPart == null)
                return new HttpNotFoundResult();

            var model = Services.ContentManager.BuildEditor(mailChimpSettingPart);
            return View(model);
        }

        [HttpPost, ActionName("EditMailChimpSetting")]
        public ActionResult EditMailChimpSettingPOST(int id, FormCollection input, string returnUrl)
        {
            var mailChimpSettingPart = _settingsService.GetSetting(id);

            var model = Services.ContentManager.UpdateEditor(mailChimpSettingPart, this);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    Services.Notifier.Error(T(error));
                }
                return View(model);
            }

            Services.Notifier.Information(T("The setting has been updated."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        public ActionResult Delete(string templateName, string returnUrl)
        {
            System.IO.File.Delete(Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/") + templateName + ".html");
            Services.Notifier.Information(T("File has been deleted."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        [HttpPost]
        public RedirectToRouteResult UploadTemplate(HttpPostedFileBase file, string templateName)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".html" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/"), templateName + fileExt);
                    file.SaveAs(path);
                    Services.Notifier.Information(T("File has been added!"));
                    return RedirectToAction("Index");
                }
            }
            Services.Notifier.Error(T("Wrong file extention!"));
            return RedirectToAction("Index");
        }

        public void Download(string fileName)
        {
            fileName += ".html";
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            Response.ContentType = "text/HTML";
            String Header = "Attachment; Filename=" + fileName;
            Response.AppendHeader("Content-Disposition", Header);
            System.IO.FileInfo Dfile = new System.IO.FileInfo(pathToTemplates+fileName);
            Response.WriteFile(Dfile.FullName);
            Response.End();
        }
        
    }
}