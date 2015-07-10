using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.FAQ.Models
{
    public class LanguageRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
    }
}