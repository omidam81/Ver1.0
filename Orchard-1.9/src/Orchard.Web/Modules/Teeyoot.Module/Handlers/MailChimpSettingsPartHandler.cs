using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;


namespace Teeyoot.Module.Handlers
{
    public class MailChimpSettingsPartHandler : ContentHandler 
    {
        public MailChimpSettingsPartHandler(IRepository<MailChimpSettingsPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnRemoved<MailChimpSettingsPart>((context, part) =>
            {
                repository.Delete(part.Record);
            });
        }
    }  
}