using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class MailTemplateSubjectRecord
    {
        public virtual int Id { get; protected set; }
        public virtual string TemplateName { get; set; }
        public virtual string Culture { get; set; }
        public virtual string Subject { get; set; }
    }
}