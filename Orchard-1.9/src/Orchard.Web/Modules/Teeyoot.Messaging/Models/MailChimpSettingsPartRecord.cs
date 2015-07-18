using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Messaging.Models
{
    public class MailChimpSettingsPartRecord : ContentPartRecord
    {
        public virtual string ApiKey { get; set; }

        public virtual string MailChimpListId { get; set; }

        public virtual string WelcomeCampaignId { get; set; }

        public virtual int WelcomeTemplateId { get; set; }

        public virtual string AllBuyersCampaignId { get; set; }

        public virtual int AllBuyersTemplateId { get; set; }

        public virtual string Culture { get; set; }
    }
}