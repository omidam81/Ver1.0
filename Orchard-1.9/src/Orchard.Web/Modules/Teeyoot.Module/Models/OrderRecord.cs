using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class OrderRecord
    {
        public virtual int Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string City { get; set; }

        public virtual string State { get; set; }

        public virtual string Country { get; set; }

        public virtual double TotalPrice { get; set; }

        public virtual CurrencyRecord CurrencyRecord { get; set; }
         
        public virtual IList<LinkOrderCampaignProductRecord> Products { get; set; }

        public OrderRecord()
        {
            Products = new List<LinkOrderCampaignProductRecord>();
        }

    }
}