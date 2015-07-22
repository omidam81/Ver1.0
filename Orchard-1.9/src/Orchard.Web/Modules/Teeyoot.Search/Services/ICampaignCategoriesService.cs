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
        IQueryable<CampaignCategoriesRecord> GetAllCategories();

        IQueryable<CampaignRecord> GetCampaignsByIdCategory(int id);

        CampaignCategoriesRecord GetCategoryById(int id);

        List<CampaignRecord> GetCampaignsByNotThisIdCategory(int id);

        int AddCategory(string name);

        bool CnehgeVisible(int id, bool changes);

        bool DeleteCategory(int id);

        bool ChnageNameCategory(int id, string newName);

        bool AddCampaignToCategory(int idCamp, int idCateg);
    }
}