using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class FontRecord
    {
        public virtual int Id { get; set; }

        public virtual string Family { get; set; }

        public virtual string FileName { get; set; }

        public virtual string Tags { get; set; }
        public virtual int? Priority { get; set; }
        public virtual string FontCulture { get; set; }
    }
}