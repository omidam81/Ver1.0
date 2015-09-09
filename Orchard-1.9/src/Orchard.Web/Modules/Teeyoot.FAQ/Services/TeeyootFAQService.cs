using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Services
{
    public class TeeyootFAQService : ITeeyootFaqService
    {
        private readonly IRepository<FaqSectionRecord> _sectionRepository;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public TeeyootFAQService(IRepository<FaqSectionRecord> sectionRepository, IContentManager contentManager, IWorkContextAccessor workContextAccessor)
        {
            _sectionRepository = sectionRepository;
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
        }

        public IEnumerable<FaqSectionRecord> GetFaqSections()
        {
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            return _sectionRepository.Table.Where(s => s.SectionCulture == cultureUsed).ToList();
        }

        public FaqSectionRecord GetDefaultSection()
        {
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            return _sectionRepository.Table.Where(s => s.SectionCulture == cultureUsed).FirstOrDefault();
        }

        public FaqSectionRecord GetFaqSectionById(int id)
        {
            return _sectionRepository.Get(id);
        }

        public FaqEntryPart CreateFaqEntry(string question, int sectionId, string languageCode, string answer = "")
        {
            var section = GetFaqSectionById(sectionId);
           
            var faqEntryPart = _contentManager.Create<FaqEntryPart>("FaqEntry",
                fe =>
                {
                    fe.Question = question;
                    fe.Answer = answer;
                    fe.Section = section;
                    fe.Language = languageCode;
                });

            return faqEntryPart;
        }

        public void DeleteFaqEntry(int id)
        {
            _contentManager.Remove(_contentManager.Get<FaqEntryPart>(id).ContentItem);
        }

        public FaqEntryPart GetFaqEntry(int id)
        {
            return _contentManager.Get<FaqEntryPart>(id, VersionOptions.Latest);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries()
        {
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            return this.GetFaqEntries(cultureUsed);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>(VersionOptions.Latest).Where(fe => fe.Language == language);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(int section)
        {
            if (section > 0)
            {
                return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>(VersionOptions.Latest).Where(fe => fe.FaqSectionRecord.Id == section);
            }
            return GetFaqEntries();
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language, int section)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>(VersionOptions.Latest).Where(fe => fe.Language == language && fe.FaqSectionRecord.Id == section);
        }
    }
}