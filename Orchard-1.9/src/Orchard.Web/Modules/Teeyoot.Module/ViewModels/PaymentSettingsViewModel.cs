using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Module.ViewModels
{
    public class PaymentSettingsViewModel
    {
       public bool CashDeliv { get; set; }
       public bool PayPal { get; set; }
       public bool Mol { get; set; }
       public bool CreditCard { get; set; }
       public bool SettingEmpty { get; set; }

       public string publicKey { get; set; }
       public string privateKey { get; set; }
       public string merchantId { get; set; }
       public string clientToken { get; set; }


       public string merchantIdMol { get; set; }
       public string verifyKey { get; set; }

       //
       //
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
    }
}