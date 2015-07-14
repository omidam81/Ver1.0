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
using Teeyoot.FAQ.Models;
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Autocomplete(string term)
        {
            var entries = _faqService.GetFaqEntries().Join<BodyPartRecord>().List();
            entries = entries.Where(s => s.Question.ToLower().Contains(term.ToLower()) || s.Body.Text.ToLower().Contains(term.ToLower())).Select(w => w).Take(7).ToList();


            var result = new List<KeyValuePair<string, string>>();
            var list = new List<SelectListItem>();

            foreach (var entr in entries)
            {
                var answer = entr.Body.Text;
                answer = System.Text.RegularExpressions.Regex.Replace(answer, "<[^>]*>", "");
                answer = System.Text.RegularExpressions.Regex.Replace(answer, "&nbsp", " ");
                var question = "<span class='autocomplete-question'>" + entr.Question + "</span><br/>";
                var text = question + answer;
                list.Add(new SelectListItem { Text = text, Value = entr.Id.ToString() });
            }

            foreach (var item in list)
            {
                result.Add(new KeyValuePair<string, string>(item.Value.ToString(), item.Text));
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailSearch(string filter)
        {
            var entries = _faqService.GetFaqEntries().Join<BodyPartRecord>().List();
            var searchResult = entries.Where(s => s.Question.ToLower().Contains(filter.ToLower()) || s.Body.Text.ToLower().Contains(filter.ToLower())).Select(w => w).ToList();
            return View(new ViewSectionViewModel { Section = new FaqSectionRecord { Name = filter }, Topics = searchResult.ToArray() });
        }
    }
}