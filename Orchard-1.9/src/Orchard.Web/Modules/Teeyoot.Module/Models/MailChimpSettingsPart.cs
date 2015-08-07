using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FAQ.Models;

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