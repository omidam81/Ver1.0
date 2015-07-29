using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public virtual string StreetAddress { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual double TotalPrice { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual DateTime? Reserved { get; set; }

        public virtual DateTime? Paid { get; set; }

        public virtual CurrencyRecord CurrencyRecord { get; set; }

        public virtual OrderStatusRecord OrderStatusRecord { get; set; }
         
        public virtual IList<LinkOrderCampaignProductRecord> Products { get; set; }

        public OrderRecord()
        {
            Products = new List<LinkOrderCampaignProductRecord>();
        }

    }
}