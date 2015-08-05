using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class LinkProductSizeRecord
    {
        public virtual int Id { get; set; }

        public virtual ProductSizeRecord ProductSizeRecord { get; set; }

        public virtual ProductRecord ProductRecord { get; set; }

        public virtual float SizeCost { get; set; }
    }
}