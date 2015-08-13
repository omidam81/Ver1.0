using System;
using System.Linq;
using System.Web;
using System.Globalization;
using Orchard;
using Orchard.Localization.Services;
using Orchard.Environment.Extensions;
using Orchard.Data;
using Orchard.Localization.Records;
using System.Collections.Generic;

namespace RM.Localization.Providers 
{
    [OrchardFeature("RM.Localization.BrowserCultureSelector")]
    public class BrowserCultureSelector : ICultureSelector {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<CultureRecord> _cultureRepository;

        public BrowserCultureSelector(IWorkContextAccessor workContextAccessor, IRepository<CultureRecord> cultureRepository)
        {
            _cultureRepository = cultureRepository;
            _workContextAccessor = workContextAccessor;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            var workContext = _workContextAccessor.GetContext();
            var cultureName = workContext != null && workContext.HttpContext != null && workContext.HttpContext.Request != null && workContext.HttpContext.Request.UserLanguages != null ? workContext.HttpContext.Request.UserLanguages.Select(x => x.Split(';').FirstOrDefault()).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) : null;

            cultureName = CultureHelper.GetSpecificCulture(cultureName);

            //if (!ListCultures().Contains(cultureName)) cultureName = null;

            return cultureName == null ? null : new CultureSelectorResult { Priority = -4, CultureName = cultureName };
        }

        private IEnumerable<string> ListCultures()
        {
            var query = from culture in _cultureRepository.Table select culture.Culture;
            return query.ToList();
        }
    }
}
