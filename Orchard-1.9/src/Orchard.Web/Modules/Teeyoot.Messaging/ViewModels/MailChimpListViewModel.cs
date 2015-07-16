using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Messaging.ViewModels
{
    public class MailChimpListViewModel
    {
        public  string ApiKey { get; set; }

        public  string MailChimpCampaignId { get; set; }

        public  int    TemplateId { get; set; }

        public  string TemplateName { get; set; }

        public  string MailChimpListId { get; set; }


    }
}