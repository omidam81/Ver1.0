using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class MailChimpSettingsPartRecord : ContentPartRecord
    {
        public virtual string ApiKey { get; set; }

        public virtual string Culture { get; set; }
    }
}