using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.Handlers
{
    public class FaqEntryPartHandler : ContentHandler 
    {
        public FaqEntryPartHandler(IRepository<FaqEntryPartRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnRemoved<FaqEntryPart>((context, part) =>
            {
                repository.Delete(part.Record);
            });
        }
    }  
}