using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class PaymentViewModel
    {
        public OrderRecord Order { get; set; }

        public string Result { get; set; }

        public string ClientToken { get; set; }
    }   
}