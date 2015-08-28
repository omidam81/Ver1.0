using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Module.ViewModels
{
    public class PaymentSettingsViewModel
    {
       public virtual LanguageRecord Culture { get; set; }
       //public int  PaumentMethod { get; set; }
       public virtual IEnumerable<LanguageRecord> Languages { get; set; }

       public bool CashDeliv { get; set; }
       public bool PayPal { get; set; }
       public bool Mol { get; set; }
       public bool CreditCard { get; set; }

       public bool SettingEmpty { get; set; }
    }
}