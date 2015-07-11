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
        private readonly ILanguageService _languageService;

        public TeeyootFAQService(IRepository<FaqSectionRecord> sectionRepository, IContentManager contentManager, ILanguageService languageService)
        {
            _sectionRepository = sectionRepository;
            _contentManager = contentManager;
            _languageService = languageService;
        }

        public IEnumerable<Models.FaqSectionRecord> GetFaqSections()
        {
            return _sectionRepository.Table.ToList();
        }

        public FaqSectionRecord GetDefaultSection()
        {
            //TODO: eugene: implement if needed
            return _sectionRepository.Table.FirstOrDefault();
        }

        public FaqSectionRecord GetFaqSectionById(int id)
        {
            return _sectionRepository.Get(id);
        }

        public FaqEntryPart CreateFaqEntry(string question, int sectionId, string languageCode, string answer = "")
        {
            var section = GetFaqSectionById(sectionId);
            var language = _languageService.GetLanguageByCode(languageCode);

            var faqEntryPart = _contentManager.Create<FaqEntryPart>("FaqEntry",
                fe =>
                {
                    fe.Question = question;
                    fe.Answer = answer;
                    fe.Section = section;
                    fe.Language = language;
                });

            return faqEntryPart;
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries()
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>();
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.LanguageRecord.Code == language);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(int section)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.FaqSectionRecord.Id == section);
        }

        public IContentQuery<FaqEntryPart> GetFaqEntries(string language, int section)
        {
            return _contentManager.Query<FaqEntryPart, FaqEntryPartRecord>().Where(fe => fe.LanguageRecord.Code == language && fe.FaqSectionRecord.Id == section);
        }
    }
}