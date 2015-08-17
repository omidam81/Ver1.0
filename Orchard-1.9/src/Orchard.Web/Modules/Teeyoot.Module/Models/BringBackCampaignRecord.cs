using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class BringBackCampaignRecord
    {
        public virtual int Id { get; set; }

        public virtual CampaignRecord CampaignRecord { get; set; }

        public virtual string Email { get; set; }       
    }
}