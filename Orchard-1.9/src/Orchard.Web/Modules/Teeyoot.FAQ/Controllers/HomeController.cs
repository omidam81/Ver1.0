using Orchard;
using Orchard.Core.Common.Models;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Services;
using Teeyoot.FAQ.ViewModels;

namespace Teeyoot.FAQ.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
        private readonly ILanguageService _languageService;
        private readonly ITeeyootFaqService _faqService;

        public HomeController(ITeeyootFaqService faqService, ILanguageService languageService)
        {
            _languageService = languageService;
            _faqService = faqService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
       



        public ActionResult Index()
        {
            var sections = _faqService.GetFaqSections();

            return View(sections);
        }


        public ActionResult ViewTopic(int topicId)
        {
            var topic = _faqService.GetFaqEntry(topicId);

            return View(topic);
        }


        public ActionResult ViewSection(int sectionId)
        {
            var section = _faqService.GetFaqSectionById(sectionId);
            var entries = _faqService.GetFaqEntries(sectionId).Join<BodyPartRecord>().List();

            return View(new ViewSectionViewModel { Section = section, Topics = entries.ToArray() });
        }
    }
}