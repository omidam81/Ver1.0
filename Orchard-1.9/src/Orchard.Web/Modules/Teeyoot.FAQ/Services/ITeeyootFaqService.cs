using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Services
{
    public interface ITeeyootFaqService : IDependency 
    {
        IEnumerable<FaqSectionRecord> GetFaqSections();
    }
}
