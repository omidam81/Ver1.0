using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.VirtualPath;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Settings;
using Orchard.Localization.Services;
using Teeyoot.FAQ.Models;
using Orchard;
using Teeyoot.FAQ.Services;
using Orchard.UI.Navigation;
using Orchard.Core.Common.Models;
using Orchard.Settings;
using Teeyoot.FAQ.ViewModels;

namespace Teeyoot.FAQ.Controllers
{
    [ValidateInput(false), Admin]
    public class FaqAdminController : Controller, IUpdateModel
    {
        private readonly ILanguageService _languageService;
        private readonly ITeeyootFaqService _faqService;
        private readonly ISiteService _siteService;

        private const string DEFAULT_LANGUAGE_CODE = "en";

        public FaqAdminController(IOrchardServices services, ILanguageService languageService, IShapeFactory shapeFactory, ITeeyootFaqService faqService, ISiteService siteService)
        {
            _languageService = languageService;
            _faqService = faqService;
            _siteService = siteService;
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }

        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        private dynamic Shape { get; set; }

        public ActionResult Index(PagerParameters pagerParameters, FaqEntrySearchViewModel search)
        {
            var sections = _faqService.GetFaqSections();
            var languages = _languageService.GetLanguages();

            if (string.IsNullOrWhiteSpace(search.LanguageCode))
            {
                var culture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var currLanguage = languages.FirstOrDefault(l => l.Code == culture);
                if (currLanguage == null)
                    currLanguage = languages.FirstOrDefault(l => l.Code == DEFAULT_LANGUAGE_CODE);

                search.LanguageCode = currLanguage.Code;
            }

            var faqQuery = _faqService.GetFaqEntries(search.SectionId).Join<BodyPartRecord>().List().Where(fe => fe.Language.Code == search.LanguageCode);

            if (!string.IsNullOrWhiteSpace(search.SearchString))
            {
                var searchString = search.SearchString.Trim();
                faqQuery = faqQuery.Where(f => f.Question.Contains(searchString) || f.Body.Text.Contains(searchString));
            }

            var entriesProjection = faqQuery.Select(e => { return Shape.FaqEntry(
                                         Id: e.Id,
                                         Question: e.Question,
                                         Text: e.Body.Text
                                    ); });
                                   
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
           
            var pagerShape = Shape.Pager(pager).TotalItemCount(faqQuery.Count());
            var model = new FaqEntriesIndexViewModel(entries, sections, languages, search, pagerShape);

            return View(model);
        }

        public ActionResult AddFaqEntry(string returnUrl)
        {
            FaqEntryPart faqEntryPart = Services.ContentManager.New<FaqEntryPart>("FaqEntry");
            if (faqEntryPart == null)
                return HttpNotFound();

            try
            {
                var culture = DEFAULT_LANGUAGE_CODE;
                var cultureInfo = CultureInfo.GetCultureInfo(culture);
                // var cultureInfo = CultureInfo.CurrentCulture; 
                // TODO: eugene: implement when supporting localization
                faqEntryPart.Language = _languageService.GetLanguageByCode(cultureInfo.Name);
                faqEntryPart.Section = _faqService.GetDefaultSection();
                var model = Services.ContentManager.BuildEditor(faqEntryPart);
                return View(model);
            }
            catch (Exception exception)
            {
                //Logger.Error(T("Creating widget failed: {0}", exception.Message).Text);
                //Services.Notifier.Error(T("Creating widget failed: {0}", exception.Message));
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            }
        }

        [HttpPost, ActionName("AddFaqEntry")]
        public ActionResult AddFaqEntryPOST([Bind(Prefix = "FaqEntryPart.SectionId")] int section,
            [Bind(Prefix = "FaqEntryPart.LanguageCode")] string language, string returnUrl)
        {
            var faqEntryPart = _faqService.CreateFaqEntry("", section, language);
            if (faqEntryPart == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(faqEntryPart, this);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Services.Notifier.Information(T("New FAQ topic has been added."));
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