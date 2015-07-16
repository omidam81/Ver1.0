using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
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

        public virtual string MailChimpCampaignId {
            get
            {
                return Retrieve(p => p.MailChimpCampaignId);
            }
            set
            {
                Store(p => p.MailChimpCampaignId, value);
            }
        
        }

        public virtual int TemplateId {
            get
            {
                return Retrieve(p => p.TemplateId);
            }
            set
            {
                Store(p => p.TemplateId, value);
            }
        }

        public virtual string TemplateName
        {
            get
            {
                return Retrieve(p => p.TemplateName);
            }
            set
            {
                Store(p => p.TemplateName, value);
            }
        }

        public virtual string MailChimpListId {
            get
            {
                return Retrieve(p => p.MailChimpListId);
            }
            set
            {
                Store(p => p.MailChimpListId, value);
            }
        }

        public virtual string Culture {
            get
            {
                return Retrieve(p => p.Culture);
            }
            set
            {
                Store(p => p.Culture, value);
            }
        }
    }

}