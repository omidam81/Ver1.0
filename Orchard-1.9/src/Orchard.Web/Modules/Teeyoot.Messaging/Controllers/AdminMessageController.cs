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

        public ActionResult Index()
        {

            //var settings = _settingsService.GetAllSettings().List().Where(s => s.Culture == "en").FirstOrDefault();

            //if (settings == null)
            //{
            //    var mailChimpSettingPart = _settingsService.CreateMailChimpSettingsPart("", "", "", 0, "", 0, "en");
            //    if (mailChimpSettingPart == null)
            //        return HttpNotFound();
            //}
            var settings = _settingsService.GetAllSettings().List().Select(s => new MailChimpSettingsPartRecord
            {
                Id = s.Id,
                ApiKey = s.ApiKey,
                MailChimpListId = s.MailChimpListId,
                WelcomeCampaignId = s.WelcomeCampaignId,
                WelcomeTemplateId = s.WelcomeTemplateId,
                AllBuyersCampaignId = s.AllBuyersCampaignId,
                AllBuyersTemplateId = s.AllBuyersTemplateId,
                Culture = s.Culture
            });

            return View(settings);
        }

        public ActionResult AddSetting(string returnUrl)
        {
            MailChimpSettingsPart mailChimpSettingsPart = Services.ContentManager.New<MailChimpSettingsPart>("MailChimpSettings");
            if (mailChimpSettingsPart == null)
                return HttpNotFound();
            try
            {
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