using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class OrderProductViewModel
    {
        public int ProductId { get; set; }

        public int Count { get; set; }

        public int SizeId { get; set; }

    }   
}