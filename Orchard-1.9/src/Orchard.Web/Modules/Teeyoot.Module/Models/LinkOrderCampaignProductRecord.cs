using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class LinkOrderCampaignProductRecord
    {
        public virtual int Id { get; set; }

        public virtual OrderRecord OrderRecord { get; set; }

        public virtual CampaignProductRecord CampaignProductRecord { get; set; }

        public virtual int Count { get; set; }

        public virtual int SizeId { get; set; }

    }
}