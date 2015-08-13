using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using RM.Localization.Models;

namespace RM.Localization.Services
{
    public interface ICookieCultureService : IDependency {
        string GetCulture();
        void SetCulture(string culture);
        void ResetCulture();
    }
}
