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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Messaging.Models;
using Teeyoot.Messaging.Services;
using Teeyoot.Messaging.ViewModels;
using Mandrill;
using Mandrill.Model;


using Teeyoot.FAQ.Services;namespace Teeyoot.Module.Controllers
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
            var settings = _settingsService.GetSettingByCulture(culture).List().Select(s => new MailChimpListViewModel
            {
                Id = s.Id,
                ApiKey = s.ApiKey,
                MailChimpListId = s.MailChimpListId,
                WelcomeCampaignId = s.WelcomeCampaignId,
                WelcomeTemplateId = s.WelcomeTemplateId,
                AllBuyersCampaignId = s.AllBuyersCampaignId,
                AllBuyersTemplateId = s.AllBuyersTemplateId,
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
            var mailChimpSettingPart = _settingsService.CreateMailChimpSettingsPart("", "", "" , 0, "", 0 , "en");
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

        public ActionResult Delete(int id, string returnUrl)
        {
            _settingsService.DeleteMailChimpSettingsPart(id);
            Services.Notifier.Information(T("The setting has been deleted."));
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
    }
}