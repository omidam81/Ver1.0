using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Services;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

namespace Teeyoot.Module.Drivers
{
    public class MailChimpSettingsPartDriver : ContentPartDriver<MailChimpSettingsPart>
    {
        private readonly IContentManager _contentManager;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly ILanguageService _languageService;
       
        public  MailChimpSettingsPartDriver( IMailChimpSettingsService settingsService, IContentManager contentManager, ILanguageService languageService)
        {
            _contentManager = contentManager;
            _settingsService = settingsService;
            _languageService = languageService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix
        {
            get { return "MailChimpSettingsPart"; }
        }

        protected override DriverResult Editor(MailChimpSettingsPart part, dynamic shapeHelper)
        {
            //part.AvailableLanguages = _languageService.GetLanguages();

            return ContentShape("Parts_MailChimpSettings_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/MailChimpSettings", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(MailChimpSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {            
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}