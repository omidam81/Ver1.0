using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class OrderHistoryRecord
    {
        public virtual int Id { get; set; }

        public virtual string Event { get; set; }

        public virtual DateTime EventDate { get; set; }

        public virtual int OrderRecord_Id { get; set; }
    }
}