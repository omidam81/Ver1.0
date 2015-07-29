﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Models
{
    public class PayoutRecord
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Event { get; set; }
        public virtual double Amount { get; set; }     
        public virtual bool IsPlus { get; set; }
        public virtual TeeyootUserPartRecord UserId { get; set; }
    }
}