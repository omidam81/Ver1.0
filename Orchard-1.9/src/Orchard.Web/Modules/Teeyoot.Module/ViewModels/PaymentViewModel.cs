﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class PaymentViewModel
    {
        public OrderRecord Order { get; set; }

        public string Result { get; set; }

        public string ClientToken { get; set; }
    }   
}