using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class PromotionViewModel
    {
        public virtual IEnumerable<PromotionRecord> Promotions { get; set; }

        public virtual string PromoId { get; set; }

        public virtual string DiscountType { get; set; }

        public virtual double AmountSize { get; set; }

        public virtual string AmountType { get; set; }

        public virtual bool Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public virtual DateTime Expiration { get; set; }

        public virtual int Redeemed { get; set; }

        public IEnumerable<SelectListItem> AvailableCurrencies { get; set; }

    }
}