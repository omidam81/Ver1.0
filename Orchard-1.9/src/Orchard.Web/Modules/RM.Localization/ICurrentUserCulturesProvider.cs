using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace RM.Localization
{
    public interface ICurrentUserCulturesProvider : IDependency
    {
        List<string> GetCulturesForCurrentUser();
    }
}
