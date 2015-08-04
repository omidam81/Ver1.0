using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class MessageRecord
    {
        public virtual int Id { get; set; }

        public virtual string Text { get; set; }

        public virtual string Sender { get; set; }

        public virtual DateTime SendDate { get; set; }

        public virtual int UserId { get; set; }

        public virtual int CampaignId { get; set; }

        public virtual string Subject { get; set; }

        public virtual bool IsApprowed { get; set; }
         
    }
}