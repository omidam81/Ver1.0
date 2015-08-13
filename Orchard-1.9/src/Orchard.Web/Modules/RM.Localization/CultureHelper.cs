using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace RM.Localization
{
    static class CultureHelper
    {
        public static string GetSpecificCulture(string cultureName) {

            try 
            {
                var ci = !string.IsNullOrEmpty(cultureName) ? CultureInfo.GetCultureInfo(cultureName) : null;

                if (ci != null && ci.IsNeutralCulture)
                {
                    ci = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(x => x.Parent.Name == ci.Name) ?? ci;
                }

                return ci == null || ci.IsNeutralCulture ? null : ci.Name;
            }
            catch {
                return null;
            }
        }
    }
}
