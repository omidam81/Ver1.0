using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class PaymentInformationRecord
    {
        public virtual int Id { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string BankName { get; set; }
        public virtual string ContactNumber { get; set; }
        public virtual string MessAdmin{ get; set; }
        public virtual string AccountHolderName{ get; set; }
        public virtual int TranzactionId { get; set; }


        
    }
}