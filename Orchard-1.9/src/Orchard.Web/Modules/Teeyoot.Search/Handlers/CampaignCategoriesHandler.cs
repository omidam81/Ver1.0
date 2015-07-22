using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using Teeyoot.Module.Models;
using Teeyoot.Search.Services;

namespace Teeyoot.Search.Handlers
{
    public class CampaignCategoriesHandler : ContentHandler
    {
        public CampaignCategoriesHandler(IRepository<CampaignCategoriesRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));

            OnRemoved<CampaignCategoriesPart>((context, part) =>
            {
                repository.Delete(part.Record);
            });
        }
    }
}