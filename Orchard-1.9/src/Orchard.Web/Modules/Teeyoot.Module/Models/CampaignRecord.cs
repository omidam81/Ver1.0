using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CampaignRecord
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }

        public virtual string Alias { get; set; }

        public virtual int ProductCountGoal { get; set; }

        public virtual int ProductCountSold { get; set; }

        [StringLengthMax]
        public virtual string Design { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual string URL { get; set; }

        public virtual bool AllowPickUpOrdersFromMe { get; set; }

        public virtual bool BackSideByDefault { get; set; }

        public virtual IList<CampaignProductRecord> Products { get; set; }

        public CampaignRecord()
        {
            Products = new List<CampaignProductRecord>();
        }
    }
}