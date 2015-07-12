using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Services;

namespace Teeyoot.FAQ.Controllers
{
    [Themed]
    public class HomeController : Controller
    {
         private readonly ILanguageService _languageService;

         public HomeController(ITeeyootFaqService services, ILanguageService languageService)
        {
            _languageService = languageService;
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

         
        private ITeeyootFaqService Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
       



        public ActionResult Index()
        {
            var sections = Services.GetFaqSectionsMoq();

            return View(sections);
        }


        public ActionResult ViewTopic(int topicId)
        {
            var topic = Services.GetFaqEntryPartRecordById(topicId);

            return View(topic);
        }


        public ActionResult ViewSection(int sectionId)
        {

            var section = Services.GetFaqSectionById(sectionId);

            return View(section);
         }

    }
}