using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    public class CookieCultureService : ICookieCultureService
    {
        private const string CookieNameTemplate = "{0}_CurrentCulture";

        private readonly IWorkContextAccessor _workContextAccessor;
        public CookieCultureService(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
        }

        private static string GetCookieName(WorkContext wc) {
            return string.Format(CookieNameTemplate, wc.CurrentSite != null && !string.IsNullOrWhiteSpace(wc.CurrentSite.SiteName) ? wc.CurrentSite.SiteName : "Site");
        }

        public string GetCulture() {
            var wc = _workContextAccessor.GetContext();
            var cookie = wc.HttpContext != null ? wc.HttpContext.Request.Cookies[GetCookieName(wc)] : null;
            return cookie != null ? cookie.Value : null;
        }

        public void SetCulture(string culture) {
            var wc = _workContextAccessor.GetContext();
            var cookieName = GetCookieName(wc);
            var cookie = new HttpCookie(cookieName) {Expires = DateTime.Now.AddYears(1), Value = culture };
            wc.HttpContext.Response.Cookies.Add(cookie);
        }

        public void ResetCulture()
        {
            var wc = _workContextAccessor.GetContext();
            var coockieName = GetCookieName(wc);
            var cookie = new HttpCookie(coockieName) { Expires = DateTime.MinValue, Value = null };
            wc.HttpContext.Response.Cookies.Add(cookie);
        }
    }
}
