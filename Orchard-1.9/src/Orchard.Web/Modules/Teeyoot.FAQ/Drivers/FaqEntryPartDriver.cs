using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Localization;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;
using Teeyoot.FAQ.Services;

namespace Teeyoot.FAQ.Drivers
{
    public class FaqEntryPartDriver : ContentPartDriver<FaqEntryPart>
    {
        private readonly ILanguageService _languageService;
        private readonly IContentManager _contentManager;
        private readonly ITeeyootFaqService _faqService;
        private readonly ICultureManager _cultureManager;

        public FaqEntryPartDriver(ILanguageService languageService, ITeeyootFaqService faqService, IContentManager contentManager, ICultureManager cultureManager)
        {
            _languageService = languageService;
            _contentManager = contentManager;
            _faqService = faqService;
            _cultureManager = cultureManager;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix
        {
            get { return "FaqEntryPart"; }
        }

        protected override DriverResult Editor(FaqEntryPart part, dynamic shapeHelper)
        {
            part.AvailableLanguages = _cultureManager.ListCultures();
            part.AvailableSections = _faqService.GetFaqSections();

            return ContentShape("Parts_FaqEntry_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/FaqEntry", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(FaqEntryPart part, IUpdateModel updater, dynamic shapeHelper)
        {            
            updater.TryUpdateModel(part, Prefix, null, null);

            if (string.IsNullOrWhiteSpace(part.Question))
            {
                updater.AddModelError("Question", T("Topic can't be empty."));
            }

            return Editor(part, shapeHelper);
        }
    }
}