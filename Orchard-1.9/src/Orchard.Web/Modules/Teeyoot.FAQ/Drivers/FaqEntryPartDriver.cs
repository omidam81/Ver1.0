using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
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
       
        public FaqEntryPartDriver(ILanguageService languageService, ITeeyootFaqService faqService, IContentManager contentManager)
        {
            _languageService = languageService;
            _contentManager = contentManager;
            _faqService = faqService;
        }

        protected override string Prefix
        {
            get { return "FaqEntryPart"; }
        }

        protected override DriverResult Editor(FaqEntryPart part, dynamic shapeHelper)
        {
            part.AvailableLanguages = _languageService.GetLanguages();
            part.AvailableSections = _faqService.GetFaqSections();

            return ContentShape("Parts_FaqEntry_Edit", () => shapeHelper
                .EditorTemplate(TemplateName: "Parts/FaqEntry", Model: part, Prefix: Prefix));
        }
    }
}