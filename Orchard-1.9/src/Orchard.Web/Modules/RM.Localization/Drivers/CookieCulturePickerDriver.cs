using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using RM.Localization.Models;
using Orchard.Localization.Services;
using RM.Localization.Services;

namespace RM.Localization.Drivers
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCulturePickerDriver : ContentPartDriver<CookieCulturePickerPart> {

        private readonly ICultureService _cultureService;
        public CookieCulturePickerDriver(ICultureService cultureService)
        {
            _cultureService = cultureService;
        }

        protected override DriverResult Display(CookieCulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_CookieCulturePicker", () => shapeHelper.Parts_CookieCulturePicker(Cultures: _cultureService.ListCultures(), CurrentCulture: _cultureService.GetCurrentCulture()));
        }
    }
}
