using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Services
{
    public interface ILanguageService : IDependency 
    {
        IEnumerable<LanguageRecord> GetLanguages();
        LanguageRecord GetLanguageByCode(string code);
    }
  
}
