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
                ChangeOrderStatusTemplate = System.IO.File.Exists(pathToTemplates + "change-order-status-template.html") ? "change-order-status-template.html" : "No file!",
                AllBuyersDeadlineTemplate = System.IO.File.Exists(pathToTemplates + "deadline-template.html") ? "deadline-template.html" : "No file!",
                WithdrawTemplate = System.IO.File.Exists(pathToTemplates + "withdraw-template.html") ? "withdraw-template.html" : "No file!",
                ConfirmOrderTemplate = System.IO.File.Exists(pathToTemplates + "confirm-order-template.html") ? "confirm-order-template.html" : "No file!",
                CampaignPromoTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promote-template.html") ? "campaign-promote-template.html" : "No file!",
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
            var mailChimpSettingPart = _settingsService.CreateMailChimpSettingsPart("", "en");
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
        
    }
}