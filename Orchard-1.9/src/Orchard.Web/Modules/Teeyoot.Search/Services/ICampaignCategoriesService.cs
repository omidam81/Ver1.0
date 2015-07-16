using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Search.Models;

namespace Teeyoot.Search.Services
{
    public interface ICampaignCategoriesService : IDependency
    {
        IQueryable<CampaignCategoriesPartRecord> GetAllCategories();
    }
}