using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class ProductColorRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual string Value { get; set; }

        public virtual int? Importance { get; set; }

        public virtual string ProdColorCulture { get; set; }
    }
}