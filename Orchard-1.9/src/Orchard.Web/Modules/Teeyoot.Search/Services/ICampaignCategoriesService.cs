using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.Services
{
    public interface ICampaignCategoriesService : IDependency
    {
        IQueryable<CampaignCategoriesPartRecord> GetAllCategories();

        IQueryable<CampaignRecord> GetCampaignsByIdCategory(int id);

        CampaignCategoriesPartRecord GetCategoryById(int id);

        IQueryable<CampaignRecord> GetCampaignsByNotThisIdCategory(int id);

        int AddCategory(string name);

        bool CnehgeVisible(int id, bool changes);

        bool DeleteCategory(int id); 
    }
}