using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Models 
{
    public class LinkStoreCampaignRecord
    {
        public virtual int Id { get; set; }

        public virtual StoreRecord StoreRecord { get; set; }

        public virtual CampaignRecord CampaignRecord { get; set; }

    }
}