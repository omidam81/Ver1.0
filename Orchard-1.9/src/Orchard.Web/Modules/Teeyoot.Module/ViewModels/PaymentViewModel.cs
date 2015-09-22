using System;
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

        public PromotionRecord Promotion { get; set; }

        public string Result { get; set; }

        public string ClientToken { get; set; }

        public bool CashDeliv { get; set; }

        public bool PayPal { get; set; }

        public bool Mol { get; set; }

        public bool CreditCard { get; set; }

        public string CountryName { get; set; }

        //
        //
        public string CashOnDeliveryAvailabilityMessage { get; set; }
        // Tab names for payment methods
        public string CashDelivTabName { get; set; }
        public string PayPalTabName { get; set; }
        public string MolTabName { get; set; }
        public string CreditCardTabName { get; set; }
        // Notes for payment methods
        public string CashDelivNote { get; set; }
        public string PayPalNote { get; set; }
        public string MolNote { get; set; }
        public string CreditCardNote { get; set; }
        //
        public string CheckoutPageRightSideContent { get; set; }
    }   
}