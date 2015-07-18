using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Messaging.Models
{
    public class MailChimpSettingsPart : ContentPart<MailChimpSettingsPartRecord>
    {
        public virtual string ApiKey
        {
            get
            {
                return Retrieve(p => p.ApiKey);
            }
            set
            {
                Store(p => p.ApiKey, value);
            }
        }

        public virtual string MailChimpListId
        {
            get
            {
                return Retrieve(p => p.MailChimpListId);
            }
            set
            {
                Store(p => p.MailChimpListId, value);
            }
        }

        public virtual string WelcomeCampaignId
        {
            get
            {
                return Retrieve(p => p.WelcomeCampaignId);
            }
            set
            {
                Store(p => p.WelcomeCampaignId, value);
            }

        }

        public virtual int WelcomeTemplateId
        {
            get
            {
                return Retrieve(p => p.WelcomeTemplateId);
            }
            set
            {
                Store(p => p.WelcomeTemplateId, value);
            }
        }

        public virtual string AllBuyersCampaignId
        {
            get
            {
                return Retrieve(p => p.AllBuyersCampaignId);
            }
            set
            {
                Store(p => p.AllBuyersCampaignId, value);
            }

        }

        public virtual int AllBuyersTemplateId
        {
            get
            {
                return Retrieve(p => p.AllBuyersTemplateId);
            }
            set
            {
                Store(p => p.AllBuyersTemplateId, value);
            }
        }

        public virtual string Culture
        {
            get
            {
                return Retrieve(p => p.Culture);
            }
            set
            {
                Store(p => p.Culture, value);
            }
        }

        [HiddenInput(DisplayValue = false)]
        public IEnumerable<LanguageRecord> AvailableLanguages { get; set; }
    }

}