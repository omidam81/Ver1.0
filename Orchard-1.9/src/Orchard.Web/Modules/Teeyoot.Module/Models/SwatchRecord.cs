using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class SwatchRecord
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual bool InStock { get; set; }

        public virtual int Red { get; set; }

        public virtual int Blue { get; set; }

        public virtual int Green { get; set; }

        public virtual string SwatchCulture { get; set; }
    }
}