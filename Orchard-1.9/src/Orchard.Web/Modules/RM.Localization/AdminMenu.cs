using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Orchard;
using Orchard.Core.Contents;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;
using Orchard.Utility.Extensions;
using RM.Localization.Services;

namespace RM.Localization
{
    [OrchardFeature("RM.Localization.AdminCookieCultureSelector")]
    public class AdminMenu : INavigationProvider
    {
        private readonly ICultureService _cultureService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public AdminMenu(ICultureService cultureService, IWorkContextAccessor workContextAccessor)
        {
            _cultureService = cultureService;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {

            var cultures = _cultureService.ListCultures();
            var currentCultureName = _cultureService.GetCurrentCulture();
            var currentCulture = cultures.FirstOrDefault(x => x.Culture == currentCultureName);

            builder.Add(T(currentCulture != null ? currentCulture.FullName : currentCultureName), "-2", menu => {
                menu.AddClass("section-dashboard");
                menu.LinkToFirstChild(false);
                foreach (var c in cultures) {
                    if (c.Culture == currentCultureName) continue;
                    menu.Add(T(c.FullName), item => item.Action("SetCulture", "CookieCulture", new { Area = "RM.Localization", culture = c.Culture, returnUrl = _workContextAccessor.GetContext().HttpContext.Request.ToUrlString() }));
                }
                menu.Add(T("Reset"), item => item.Action("ResetCulture", "CookieCulture", new { Area = "RM.Localization", returnUrl = _workContextAccessor.GetContext().HttpContext.Request.ToUrlString() }));
            }, null);

        }
    }
}
