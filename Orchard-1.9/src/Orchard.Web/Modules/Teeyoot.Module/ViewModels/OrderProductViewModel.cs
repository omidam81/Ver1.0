using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class OrderProductViewModel
    {
        public int ProductId { get; set; }

        public int Count { get; set; }

        public int SizeId { get; set; }

        public int Price { get; set; }

        public int ColorId { get; set; }

    }   
}