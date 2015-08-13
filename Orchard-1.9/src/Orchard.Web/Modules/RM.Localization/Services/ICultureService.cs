using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RM.Localization.Models;
using Orchard;

namespace RM.Localization.Services
{
    public interface ICultureService : IDependency {
        IEnumerable<CultureItemModel> ListCultures();
        string GetCurrentCulture();
    }
}
