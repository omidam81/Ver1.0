using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class LinkProductColorRecord
    {
        public virtual int Id { get; set; }

        public virtual ProductColorRecord ProductColorRecord { get; set; }

        public virtual ProductRecord ProductRecord { get; set; }

        //public virtual double BaseCost { get; set; }
    }
}