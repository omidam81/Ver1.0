using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using System.Globalization;
using RM.Localization.Services;

namespace RM.Localization.Providers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureSelector : ICultureSelector
    {
        private readonly ICookieCultureService _cookieCultureService;
        public CookieCultureSelector(ICookieCultureService cookieCultureService)
        {
            _cookieCultureService = cookieCultureService;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            
            var cultureCookie = _cookieCultureService.GetCulture();
            if(cultureCookie == null) return null;

            var cultureName = CultureHelper.GetSpecificCulture(cultureCookie);

            return cultureName == null ? null : new CultureSelectorResult { Priority = 0, CultureName = cultureName };
        }
    }
}
