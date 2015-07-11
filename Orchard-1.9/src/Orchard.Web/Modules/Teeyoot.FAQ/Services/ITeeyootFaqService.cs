using Orchard;
using Orchard.ContentManagement;
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
        
        IEnumerable<FaqSectionRecord> GetFaqSectionsMoq();

        FaqEntryPartRecord GetFaqEntryPartRecordById(int id);

        FaqSectionRecord GetFaqSectionRecordById(int id);
        
        FaqSectionRecord GetDefaultSection();
        FaqEntryPart CreateFaqEntry(string question, int sectionId, string languageCode, string answer = "");
        IContentQuery<FaqEntryPart> GetFaqEntries();

        IContentQuery<FaqEntryPart> GetFaqEntries(string language);

        IContentQuery<FaqEntryPart> GetFaqEntries(int section);

        IContentQuery<FaqEntryPart> GetFaqEntries(string language, int section);
    }
}
