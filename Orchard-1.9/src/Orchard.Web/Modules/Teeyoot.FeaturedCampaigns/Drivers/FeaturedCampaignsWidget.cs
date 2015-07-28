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

        protected override DriverResult Display(FeaturedCampaignsWidgetPart part, string displayType, dynamic shapeHelper)
        {
            var campaignsInFeatured = _featuredCampaignsService.GetCampaignsFromAdmin().OrderByDescending(c => c.ProductCountSold).ToList();
            var featuredCampaigns = new List<CampaignRecord>();

            if (campaignsInFeatured.Count >= 6)
            {
                featuredCampaigns = campaignsInFeatured;
            }
            else
            {
                featuredCampaigns = campaignsInFeatured;
                int countTopCamp = 6 - campaignsInFeatured.Count;
                var ordersFromOneDay = _featuredCampaignsService.GetOrderForOneDay();
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
                        featuredCampaigns.Add(campaignsFromOrderForDay.ElementAt(rand.Next(take)).Key);
                    }
                }
                
                if (featuredCampaigns.Count() < 6)
                {
                    countTopCamp = 6 - featuredCampaigns.Count();
                    var otherCampaigns = _featuredCampaignsService.GetAllCampaigns().ToList();
                    foreach (var camp in campaignsInFeatured)
                    {
                        if (otherCampaigns.Exists(c => c.Id == camp.Id) != null)
                        {
                            otherCampaigns.Remove(camp);
                        }
                    }
                    int max = otherCampaigns.Count();

                    Random rand = new Random();
                    for (int i = 0; i < countTopCamp; i++)
                    {
                        featuredCampaigns.Add(otherCampaigns.ElementAt(rand.Next(max)));
                    }
                }
            }

            return ContentShape("Parts_FeaturedCampaignsWidget", () => shapeHelper.Parts_FeaturedCampaignsWidget(Campaigns: featuredCampaigns));
        }
    }
}