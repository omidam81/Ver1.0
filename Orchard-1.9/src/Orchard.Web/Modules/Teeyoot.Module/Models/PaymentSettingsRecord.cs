﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class PaymentSettingsRecord
    {
        public virtual int Id { get; set; }
        public virtual string Culture { get; set; }
        //public virtual int PaymentMethod { get; set; }
        public virtual int Environment { get; set; }
        public virtual string PublicKey { get; set; }
        public virtual string PrivateKey { get; set; }
        public virtual string MerchantId { get; set; }
        public virtual string ClientToken { get; set; }
      

        //Payment Method
        public virtual bool CashDeliv{ get; set; }
        public virtual bool PayPal { get; set; }
        public virtual bool Mol { get; set; }
        public virtual bool CreditCard { get; set; }

        public virtual string MerchantIdMol { get; set; }
        public virtual string VerifyKey { get; set; }

    }
}