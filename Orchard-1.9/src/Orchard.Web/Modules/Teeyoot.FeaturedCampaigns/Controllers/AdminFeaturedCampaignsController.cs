using Orchard;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.FeaturedCampaigns.Services;
using Teeyoot.Module.Models;
using Teeyoot.FeaturedCampaigns.ViewModels;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminFeaturedCampaignsController : Controller
    {
        private readonly IFeaturedCampaignsService _featuredCampaignsService;
        private readonly ISiteService _siteService;
        private IOrchardServices Services { get; set; }
        private dynamic Shape { get; set; }

        public AdminFeaturedCampaignsController(IFeaturedCampaignsService featuredCampaignsService, ISiteService siteService, IShapeFactory shapeFactory, IOrchardServices services)
        {
            _featuredCampaignsService = featuredCampaignsService;
            _siteService = siteService;
            Shape = shapeFactory;
            Services = services;
        }

        // GET: Admin
        public ActionResult Index()
        {
            var campaignsInFeatured = _featuredCampaignsService.GetCampaignsFromAdmin();
            var ordersFromOneDay = _featuredCampaignsService.GetOrderForOneDay();

            var otherCampaigns = _featuredCampaignsService.GetAllCampaigns().ToList();

            Dictionary<CampaignRecord, int> allCampaigns = new Dictionary<CampaignRecord, int>();
            List<int> color =  new List<int>();

            Dictionary<CampaignRecord, int> campaignsFromOrderForDay = new Dictionary<CampaignRecord, int>();
            int[] ordersIdFromOneDay;
            if (ordersFromOneDay != null && ordersFromOneDay.Count > 0)
            {
                ordersIdFromOneDay = ordersFromOneDay.Select(c => c.Id).ToArray();
                campaignsFromOrderForDay = _featuredCampaignsService.GetCampaignsFromOrderForOneDay(ordersIdFromOneDay);

                foreach (var camp in campaignsFromOrderForDay)
                {
                    if (otherCampaigns.Exists(c => c.Id == camp.Key.Id) != null)
                    {
                        otherCampaigns.Remove(camp.Key);
                    }
                }

                campaignsFromOrderForDay.OrderByDescending(c => c.Value).OrderBy(c => c.Key.Title);
                foreach (var camp in campaignsFromOrderForDay)
                {
                    allCampaigns.Add(camp.Key, camp.Value);
                    color.Add(2);
                }
            }

            Dictionary<CampaignRecord, int> campaignsInFeaturedForDay = new Dictionary<CampaignRecord, int>();
            if (campaignsInFeatured != null && campaignsInFeatured.Count > 0)
            {
                campaignsInFeaturedForDay = _featuredCampaignsService.GetCampaignsFromAdminForOneDay(campaignsInFeatured);
                campaignsInFeaturedForDay.OrderByDescending(c => c.Value).OrderBy(c => c.Key.Title);
                foreach (var camp in campaignsInFeaturedForDay)
                {
                    if (otherCampaigns.Exists(c => c.Id == camp.Key.Id) != null)
                    {
                        otherCampaigns.Remove(camp.Key);
                    }
                }

                foreach (var camp in campaignsInFeaturedForDay)
                {
                    allCampaigns.Add(camp.Key, camp.Value);
                    color.Add(1);
                }
            }

            otherCampaigns.OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title);

            foreach (var camp in otherCampaigns)
            {
                allCampaigns.Add(camp, 0);
                color.Add(0);
            }

            return View("Index", new AdminFeaturedCampaignsViewModel { AllInFeatured = allCampaigns, Color = color });
        }
    }
}