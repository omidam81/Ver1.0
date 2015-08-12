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
       public int  PaumentMethod { get; set; }
       public virtual IEnumerable<LanguageRecord> Languages { get; set; }
    }
}