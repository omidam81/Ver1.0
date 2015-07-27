using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.FeaturedCampaigns.Services;
using Orchard.ContentManagement.Drivers;

namespace Teeyoot.FeaturedCampaigns.Drivers
{
    public class FeaturedCampaignsWidget : ContentPartDriver<FeaturedCampaignsWidgetPart>
    {
        private readonly IFeaturedCampaignsService _featuredCampaignsService;

        public FeaturedCampaignsWidget(IFeaturedCampaignsService featuredCampaignsService)
        {
            _featuredCampaignsService = featuredCampaignsService;
        }
    }
}