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
            return View("Index");
        }
    }
}