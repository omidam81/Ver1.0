using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface IMailSubjectService
    {
        string GetMailSubject(string templateName, string culture);
    }
}
