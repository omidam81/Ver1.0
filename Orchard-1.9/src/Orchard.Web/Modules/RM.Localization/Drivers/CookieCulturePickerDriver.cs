using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
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
        private readonly ICurrentUserCulturesProvider _curentUserCulturesProvider;

        public CookieCulturePickerDriver(ICultureService cultureService, ICurrentUserCulturesProvider curentUserCulturesProvider)
        {
            _cultureService = cultureService;
            _curentUserCulturesProvider = curentUserCulturesProvider;
        }

        protected override DriverResult Display(CookieCulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            var userCultures = _curentUserCulturesProvider.GetCulturesForCurrentUser();
            var l = _cultureService.ListCultures().Where(c => userCultures.Contains(c.Culture));

            var result = ContentShape("Parts_CookieCulturePicker",
                    () => shapeHelper.Parts_CookieCulturePicker(
                        Cultures: l, CurrentCulture: _cultureService.GetCurrentCulture()
                        )
                );
            return result;

            //var result = ContentShape("Parts_CookieCulturePicker",
            //        () => shapeHelper.Parts_CookieCulturePicker(
            //            Cultures: _cultureService.ListCultures(), CurrentCulture: _cultureService.GetCurrentCulture()
            //            )
            //    );
        }
    }
}
