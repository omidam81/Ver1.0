using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization.Records;
using Orchard.Localization.Services;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    [OrchardSuppressDependency("Orchard.Localization.Services.DefaultCultureManager")]
    public class ShadowCultureManager : ICultureManager
    {
        private readonly ICultureManager _underlyingCultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ShadowCultureManager(IRepository<CultureRecord> cultureRepository, 
                                     IEnumerable<ICultureSelector> cultureSelectors, 
                                     ISignals signals, 
                                     IWorkContextAccessor workContextAccessor,
                                    ICacheManager cacheManager) {
            _workContextAccessor = workContextAccessor;
            _underlyingCultureManager = 
                new DefaultCultureManager(cultureRepository, signals, workContextAccessor, cacheManager);
        }

        public IEnumerable<string> ListCultures() {
            return _underlyingCultureManager.ListCultures();
        }

        public void AddCulture(string cultureName)
        {
            _underlyingCultureManager.AddCulture(cultureName);
        }

        public void DeleteCulture(string cultureName)
        {
            _underlyingCultureManager.DeleteCulture(cultureName);
        }

        public string GetCurrentCulture(System.Web.HttpContextBase requestContext) 
        {
            var cultureName = _underlyingCultureManager.GetCurrentCulture(requestContext);
            return LookupForMasterCulture(cultureName);
        }

        private string LookupForMasterCulture(string cultureName)
        {
            // If we found culture in supported - just return it
            if (ListCultures().Contains(cultureName, StringComparer.InvariantCultureIgnoreCase)) return cultureName;
            // Else try to find Master culture

            var wc = _workContextAccessor.GetContext();
            var part = wc.CurrentSite.As<ShadowCulturePart>();

            if (part == null || string.IsNullOrWhiteSpace(part.Rules)) return cultureName;

            var masterCulture = part.Rules.Split('\r', '\n').Where(s => !string.IsNullOrWhiteSpace(s) && s.IndexOf(":") > 0).Select(s => new KeyValuePair<string, string>(s.Substring(0, s.IndexOf(":")), s.Substring(s.IndexOf(":") + 1))).Where(p => !string.IsNullOrWhiteSpace(p.Value) && new Regex(p.Value, RegexOptions.IgnoreCase).IsMatch(cultureName)).Select(p=>p.Key).FirstOrDefault();

            return masterCulture ?? GetSiteCulture();
        }

        public Orchard.Localization.Records.CultureRecord GetCultureById(int id) {
            return _underlyingCultureManager.GetCultureById(id);
        }

        public Orchard.Localization.Records.CultureRecord GetCultureByName(string cultureName) {
            return _underlyingCultureManager.GetCultureByName(cultureName);
        }

        public string GetSiteCulture() {
            return _underlyingCultureManager.GetSiteCulture();
        }

        public bool IsValidCulture(string cultureName) {
            return _underlyingCultureManager.IsValidCulture(cultureName);
        }
    }
}
