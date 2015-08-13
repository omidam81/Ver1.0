using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.FAQ.Models
{
    public class FaqEntryPartRecord : ContentPartRecord
    {
        public virtual string Question { get; set; }
        public virtual string Answer { get; set; }
        public virtual string Language { get; set; }
        public virtual FaqSectionRecord FaqSectionRecord { get; set; }
    }
}