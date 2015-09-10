using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.FAQ.Models
{
    public class FaqSectionRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string SectionCulture { get; set; }

        public virtual IList<FaqEntryPartRecord> Entries { get; set; }

        public FaqSectionRecord()
        {
            Entries = new List<FaqEntryPartRecord>();
        }
    }
}