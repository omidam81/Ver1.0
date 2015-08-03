using Orchard;
using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Services
{
    public interface ICampaignCategoriesService : IDependency
    {
        IQueryable<CampaignCategoriesRecord> GetAllCategories();

        IQueryable<CampaignCategoriesRecord> GetCategoryByCampaignId(int id);

        bool CreateCategoriesToCampaign(List<CampaignCategoriesRecord> categories, int campaignId);

        bool UpdateCampaignAndCreateNewCategories(CampaignRecord campaign, List<CampaignCategoriesRecord> newCategories, List<CampaignCategoriesRecord> categoriesInTable);
    }
}