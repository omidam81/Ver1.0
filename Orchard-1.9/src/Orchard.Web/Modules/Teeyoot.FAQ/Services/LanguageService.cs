using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<LanguageRecord> _languageRepository;

        public LanguageService(IRepository<LanguageRecord> languageRepository)
        {
            _languageRepository = languageRepository;
        }

        public IEnumerable<LanguageRecord> GetLanguages()
        {
            return _languageRepository.Table.ToList();
        }

        public LanguageRecord GetLanguageByCode(string code)
        {
            return _languageRepository.Table.FirstOrDefault(l => l.Code == code);
        }
    }
}