using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class StoreRecord
    {
        public virtual int Id { get; set; }

        public virtual int? TeeyootUserId { get; set; }

        public virtual string Title { get; set; }

        public virtual string Url { get; set; }

        public virtual string Description { get; set; }

        public virtual bool HideStore { get; set; }

        public virtual bool CrossSelling { get; set; }

        public virtual IList<LinkStoreCampaignRecord> Campaigns { get; set; }

        public StoreRecord()
        {
            Campaigns = new List<LinkStoreCampaignRecord>();
        }
    }
}