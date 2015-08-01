﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Teeyoot.Module.Models
{
    public class PromotionRecord
    {
        public virtual int Id { get; set; }
        [Required]
        public virtual string PromoId { get; set; }

        public virtual string DiscountType { get; set; }
        [Required]
        public virtual int AmountSize { get; set; }

        public virtual string AmountType { get; set; }

        public virtual bool Status { get; set; }

        public virtual DateTime Expiration { get; set; }

        public virtual int Redeemed { get; set; }

        public virtual int UserId { get; set; }
    }
}