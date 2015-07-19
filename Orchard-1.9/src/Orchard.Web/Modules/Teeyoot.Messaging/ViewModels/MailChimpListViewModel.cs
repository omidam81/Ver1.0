using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Messaging.ViewModels
{
    public class MailChimpListViewModel
    {
        public virtual int Id { get; set; } 
        
        public virtual string ApiKey { get; set; }

        public virtual string MailChimpListId { get; set; }

        public virtual string WelcomeCampaignId { get; set; }

        public virtual int WelcomeTemplateId { get; set; }

        public virtual string AllBuyersCampaignId { get; set; }

        public virtual int AllBuyersTemplateId { get; set; }

        public virtual string Culture { get; set; }

        public virtual IEnumerable<LanguageRecord> AvailableLanguages { get; set; }


    }
}