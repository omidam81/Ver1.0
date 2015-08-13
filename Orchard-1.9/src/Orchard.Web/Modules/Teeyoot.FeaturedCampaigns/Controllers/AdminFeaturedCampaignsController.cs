﻿using Orchard;
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
using Orchard.UI.Navigation;
using Orchard.Localization;
using Teeyoot.Module.Services;
using Orchard.Logging;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminFeaturedCampaignsController : Controller
    {
        private readonly ISiteService _siteService;
        private readonly ICampaignService _campaignService;
        private readonly IOrderService _orderService;
        private IOrchardServices Services { get; set; }
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public AdminFeaturedCampaignsController(ISiteService siteService, 
                                                IShapeFactory shapeFactory, 
                                                IOrchardServices services, 
                                                ICampaignService campaignService,
                                                IOrderService orderService,
                                                ITeeyootMessagingService teeyootMessagingService)
        {
            _siteService = siteService;
            _campaignService = campaignService;
            _orderService = orderService;
            _teeyootMessagingService = teeyootMessagingService;
            Shape = shapeFactory;
            Services = services;
            Logger = NullLogger.Instance;
        }

        // GET: Admin
        public ActionResult Index(PagerParameters pagerParameters)
        {
            var campaigns = _campaignService.GetAllCampaigns();
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday);

            var featuredCampaigns = new FeaturedCampaignViewModel[] { };

            var total =_campaignService.GetAllCampaigns().Count();

            if (total > 0)
            {
                featuredCampaigns = campaigns
                    .Select(c => new CampaignViewModel
                    { 
                        Id = c.Id,
                        Goal = c.ProductCountGoal,
                        Sold = c.ProductCountSold,
                        IsFeatured = c.IsFeatured,
                        Title = c.Title,
                        IsActive  = c.IsActive,
                        Alias = c.Alias,
                        CreatedDate = c.StartDate.ToLocalTime(),
                        IsApproved = c.IsApproved,
                        Minimum = c.ProductMinimumGoal
                    })
                    .Select(c => new FeaturedCampaignViewModel
                    {
                        Campaign = c,
                        Last24HoursSold =
                                    last24hoursOrders
                                        .SelectMany(o => o.Products)
                                        .Where(p => p.CampaignProductRecord.CampaignRecord_Id == c.Id)
                                        .Sum(p => (int?)p.Count) ?? 0
                    })
                    .OrderByDescending(c => c.Campaign.IsFeatured)
                    .OrderByDescending(c => c.Last24HoursSold)                   
                    .ToArray();
            }
          
            return View("Index", new AdminFeaturedCampaignsViewModel { Campaigns = featuredCampaigns });
        }

        public ActionResult ChangeVisible(PagerParameters pagerParameters, int id, bool visible)
        {
            var featuredCampaigns = _campaignService.GetAllCampaigns().Where(c => c.IsFeatured);

            if (featuredCampaigns.Count() >= 6 && visible)
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Can not update campaign, because already selected 6 companies!"));
            }
            else
            {
                var campUpdate = _campaignService.GetCampaignById(id);
                campUpdate.IsFeatured = visible;

                try
                {
                    _campaignService.UpdateCampaign(campUpdate);
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The campaign has successfully updated."));
                }
                catch (Exception e)
                {
                    Logger.Error("Error when tring to update campaign ----------------------------> " + e.ToString());
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Can not update campaign. Try again later!"));
                }
            }
            return RedirectToAction("Index", new { PagerParameters = pagerParameters });
        }

        public ActionResult DeleteCampaign(PagerParameters pagerParameters, int id)
        {
            if (_campaignService.DeleteCampaign(id))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The campaign was deleted successfully!"));
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("The company could not be removed. Try again!"));
            }

            return this.RedirectToAction("Index", new { pagerParameters = pagerParameters});
        }

        public ActionResult Approve(PagerParameters pagerParameters, int id)
        {
            var campaign = _campaignService.GetCampaignById(id);
            campaign.IsApproved = true;
            _campaignService.UpdateCampaign(campaign);
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _teeyootMessagingService.SendLaunchCampaignMessage(pathToTemplates, pathToMedia, campaign.Id);
            return RedirectToAction("Index", new { PagerParameters = pagerParameters });
        }
    }
}