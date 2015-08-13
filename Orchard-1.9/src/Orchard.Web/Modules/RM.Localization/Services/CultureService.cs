using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    public class CultureService : ICultureService
    {
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CultureService(IWorkContextAccessor workContextAccessor, ICultureManager cultureManager)
        {
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<CultureItemModel> ListCultures()
        {
            return _cultureManager.ListCultures().Select(x => new CultureInfo(x)).Select(x => new CultureItemModel { Culture = x.Name, LocalizedName = x.NativeName, ShortName = x.ThreeLetterISOLanguageName, FullName = x.DisplayName });
        }

        public string GetCurrentCulture()
        {
            return _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
        }
    }
}
