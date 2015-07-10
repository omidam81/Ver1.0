using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class ProductGroupRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<ProductRecord> Products { get; private set; }

        public ProductGroupRecord()
        {
            Products = new List<ProductRecord>();
        }
    }
}