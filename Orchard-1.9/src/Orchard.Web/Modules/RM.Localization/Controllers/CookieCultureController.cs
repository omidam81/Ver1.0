using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Orchard.Themes;
using RM.Localization.Services;

namespace RM.Localization.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureController : Controller
    {
        private readonly ICookieCultureService _cookieCultureService;
        public CookieCultureController(ICookieCultureService cookieCultureService)
        {
            _cookieCultureService = cookieCultureService;
        }

        [HttpGet]
        public ActionResult SetCulture(string culture, string returnUrl) {
            _cookieCultureService.SetCulture(culture);
            return this.RedirectLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult ResetCulture(string returnUrl)
        {
            _cookieCultureService.ResetCulture();
            return this.RedirectLocal(returnUrl);
        }
    }
}
