using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.Data;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Services
{
    public class MailSubjectService : IMailSubjectService
    {
        private readonly IRepository<MailTemplateSubjectRecord> _subjectRepository;

        public MailSubjectService(IRepository<MailTemplateSubjectRecord> subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public string GetMailSubject(string templateName, string culture)
        {
            var subject = _subjectRepository.Table
                .First(s => s.TemplateName == templateName && s.Culture == culture);

            return subject.Subject;
        }
    }
}