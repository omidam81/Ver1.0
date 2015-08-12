using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class PaymentSettingsRecord
    {
        public virtual int Id { get; set; }
        public virtual string Culture { get; set; }
        public virtual int PaymentMethod { get; set; }
    }
}