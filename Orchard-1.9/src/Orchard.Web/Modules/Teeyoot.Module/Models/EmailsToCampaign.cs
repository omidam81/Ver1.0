using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class EmailsToCampaign
    {
        public virtual int Id { get; set; }

        public virtual int CampaignRecord_Id { get; set; }

        public virtual string Email { get; set; }
    }
}