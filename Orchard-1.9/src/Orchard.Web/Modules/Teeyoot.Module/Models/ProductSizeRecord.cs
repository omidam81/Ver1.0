using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class ProductSizeRecord
    {
        public virtual int Id { get; set; }

        public virtual SizeCodeRecord SizeCodeRecord { get; set; }

        public virtual double LengthCm { get; set; }

        public virtual double WidthCm { get; set; }

        public virtual double? SleeveCm { get; set; }

        public virtual double LengthInch { get; set; }

        public virtual double WidthInch { get; set; }

        public virtual double? SleeveInch { get; set; }
    }
}