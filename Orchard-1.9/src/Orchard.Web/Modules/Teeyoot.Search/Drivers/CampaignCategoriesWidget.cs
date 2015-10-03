using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement.Drivers;
using Teeyoot.Module.Models;
using Teeyoot.Search.Services;
using System.Web.Mvc;
using Teeyoot.Module.Services.Interfaces;
using Orchard;

namespace Teeyoot.Search.Drivers
{
    public class CampaignCategoriesWidget : ContentPartDriver<CampaignCategoriesWidgetPart>
    {
        private readonly ICampaignCategoriesService _campaignCategory;

        public CampaignCategoriesWidget(ICampaignCategoriesService campaignCategory)
        {
            _campaignCategory = campaignCategory;
        }

        protected override DriverResult Display(CampaignCategoriesWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_CampaignCategoriesWidget", () => shapeHelper.Parts_CampaignCategoriesWidget(Providers: _campaignCategory.GetAllCategories().ToList()));
        }
    }
}
