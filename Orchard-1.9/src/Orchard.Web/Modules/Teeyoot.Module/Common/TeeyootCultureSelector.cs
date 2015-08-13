using Orchard;
using Orchard.Localization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Common
{
    public class TeeyootCultureSelector : ICultureSelector
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public TeeyootCultureSelector(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        public CultureSelectorResult GetCulture(HttpContextBase context)
        {
            var culture = _workContextAccessor.GetContext();

            return null;
            //return new CultureSelectorResult { CultureName = "ru-RU", Priority = 5 };
        }
    }
}