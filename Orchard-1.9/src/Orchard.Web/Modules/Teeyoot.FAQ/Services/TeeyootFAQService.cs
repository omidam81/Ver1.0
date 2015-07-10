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

        public TeeyootFAQService(IRepository<FaqSectionRecord> sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        public IEnumerable<Models.FaqSectionRecord> GetFaqSections()
        {
            return _sectionRepository.Table.ToList();
        }
    }
}