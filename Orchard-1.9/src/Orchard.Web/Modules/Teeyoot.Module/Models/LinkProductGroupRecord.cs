using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class LinkProductGroupRecord
    {
        public virtual int Id { get; set; }

        public virtual ProductGroupRecord ProductGroupRecord { get; set; }

        public virtual ProductRecord ProductRecord { get; set; }
    }
}