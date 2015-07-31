﻿using System;
using System.Linq;

namespace Teeyoot.Module.Models
{
    public class PromotionRecord
    {
        public virtual int Id { get; set; }
        
        public virtual string PromoId { get; set; }

        public virtual string DiscountType { get; set; }

        public virtual int AmountSize { get; set; }

        public virtual string AmountType { get; set; }

        public virtual bool Status { get; set; }

        public virtual DateTime Expiration { get; set; }

        public virtual int Redeemed { get; set; }
    }
}