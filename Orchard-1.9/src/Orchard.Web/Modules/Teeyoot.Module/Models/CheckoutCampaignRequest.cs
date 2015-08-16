using System;

namespace Teeyoot.Module.Models
{
    public class CheckoutCampaignRequest
    {
        public virtual int Id { get; protected set; }
        public virtual DateTime RequestUtcDate { get; set; }
        public virtual bool EmailSent { get; set; }
        public virtual DateTime? EmailSentUtcDate { get; set; }
    }
}