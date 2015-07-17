using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class ProductRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual ProductHeadlineRecord ProductHeadlineRecord { get; private set; }

        public virtual ProductImageRecord ProductImageRecord { get; set; }

        [StringLengthMax]
        public virtual string Details { get; set; }
        public virtual string Materials { get; set; }

        public virtual IList<LinkProductColorRecord> ColorsAvailable { get; set; }

        public virtual IList<LinkProductSizeRecord> SizesAvailable { get; set; }

        public ProductRecord()
        {
            ColorsAvailable = new List<LinkProductColorRecord>();
            SizesAvailable = new List<LinkProductSizeRecord>();
        }
    }
}