using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.FeaturedCampaigns.Services;
using Orchard.ContentManagement.Drivers;
using Teeyoot.Module.Services;
using Orchard;

namespace Teeyoot.FeaturedCampaigns.Drivers
{
    public class FeaturedCampaignsWidget : ContentPartDriver<FeaturedCampaignsWidgetPart>
    {
        private readonly ICampaignService _campaignsService;
        private readonly IFeaturedCampaignsService _featuredCampaignsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public FeaturedCampaignsWidget(ICampaignService campaignsService, IFeaturedCampaignsService featuredCampaignsService, IWorkContextAccessor workContextAccessor)
        {
            _campaignsService = campaignsService;
            _featuredCampaignsService = featuredCampaignsService;

            _workContextAccessor = workContextAccessor;
        }

        protected override DriverResult Display(FeaturedCampaignsWidgetPart part, string displayType, dynamic shapeHelper)
        {
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            var campaignsInFeatured = _campaignsService.GetAllCampaigns().Where(c => c.IsFeatured && !c.IsPrivate && c.IsActive && c.IsApproved && c.CampaignCulture == cultureUsed).OrderByDescending(c => c.ProductCountSold).ToList();
            var featuredCampaigns = new List<CampaignRecord>();

            if (campaignsInFeatured.Count >= 6)
            {
                featuredCampaigns = campaignsInFeatured;
            }
            else
            {
                featuredCampaigns = campaignsInFeatured;
                int countTopCamp = 6 - campaignsInFeatured.Count;
                var ordersFromOneDay = _featuredCampaignsService.GetOrderForOneDay().Where(c => c.Products.First().CampaignProductRecord.CurrencyRecord.CurrencyCulture == cultureUsed).ToList();
                if (ordersFromOneDay != null && ordersFromOneDay.Count > 0)
                {
                    int[] ordersIdFromOneDay = ordersFromOneDay.Select(c => c.Id).ToArray();
                    Dictionary<CampaignRecord, int> campaignsFromOrderForDay = _featuredCampaignsService.GetCampaignsFromOrderForOneDay(ordersIdFromOneDay);

                    int take = campaignsFromOrderForDay.Count > 12 ? 12 : campaignsFromOrderForDay.Count;
                    campaignsFromOrderForDay = campaignsFromOrderForDay.OrderByDescending(c => c.Value).OrderBy(c => c.Key.Title).Skip(0).Take(take).ToDictionary(p => p.Key, p => p.Value);

                    Random rand = new Random();
                    int insertCamp = campaignsFromOrderForDay.Count() <= countTopCamp ? campaignsFromOrderForDay.Count() : countTopCamp;
                    for (int i = 0; i < insertCamp; i++)
                    {
                        var campNum = rand.Next(take);
                        var campKey = campaignsFromOrderForDay.ElementAt(campNum).Key;
                        if (!featuredCampaigns.Contains(campKey))
                        {
                            featuredCampaigns.Add(campKey);
                        }

                    }
                }
                
                if (featuredCampaigns.Count() < 6)
                {
                    countTopCamp = 6 - featuredCampaigns.Count();
                    var otherCampaigns = _campaignsService.GetAllCampaigns().Where(c => !c.IsPrivate && c.IsActive && c.IsApproved && c.CampaignCulture == cultureUsed).ToList();
                    foreach (var camp in campaignsInFeatured)
                    {
                        if (otherCampaigns.Exists(c => c.Id == camp.Id))
                        {
                            otherCampaigns.Remove(camp);
                        }
                    }
                    int max = otherCampaigns.Count();
                    if ((max + featuredCampaigns.Count()) < 6)
                    {
                        featuredCampaigns = null;
                    }
                    else
                    {

                        Random rand = new Random();
                        for (int i = 0; i < countTopCamp; i++)
                        {
                            var res = false;
                            while (!res)
                            {
                                var camp = otherCampaigns.ElementAt(rand.Next(max));
                                if (!featuredCampaigns.Exists(c => c.Id == camp.Id))
                                {
                                    featuredCampaigns.Add(camp);
                                    res = true;
                                }
                            }
                        }
                    }
                }
            }

            return ContentShape("Parts_FeaturedCampaignsWidget", () => shapeHelper.Parts_FeaturedCampaignsWidget(Campaigns: featuredCampaigns));
        }
    }
}