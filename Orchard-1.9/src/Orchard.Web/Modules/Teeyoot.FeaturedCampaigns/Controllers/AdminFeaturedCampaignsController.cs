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
using Orchard.UI.Navigation;
using Orchard.Localization;
using Teeyoot.Module.Services;
using Orchard.Logging;
using Teeyoot.Module.Common.Enums;
using Orchard.Data;

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
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public ILogger Logger { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        public AdminFeaturedCampaignsController(ISiteService siteService, 
                                                IShapeFactory shapeFactory, 
                                                IOrchardServices services, 
                                                ICampaignService campaignService,
                                                IOrderService orderService,
                                                ITeeyootMessagingService teeyootMessagingService,
                                                IRepository<CurrencyRecord> currencyRepository,
                                                IWorkContextAccessor workContextAccessor)
        {
            _siteService = siteService;
            _campaignService = campaignService;
            _orderService = orderService;
            _teeyootMessagingService = teeyootMessagingService;
            _currencyRepository = currencyRepository;
            Shape = shapeFactory;
            Services = services;
            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        // GET: Admin
        public ActionResult Index(PagerParameters pagerParameters, int? filterCurrencyId = null)
        {
            var campaigns = _campaignService.GetAllCampaigns().Where(c => c.CampaignCulture == cultureUsed).
                Where(c => (null == filterCurrencyId) || (c.CurrencyRecord.Id == filterCurrencyId));
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var last24hoursOrders = _orderService.GetAllOrders().Where(o => o.IsActive && o.Created >= yesterday && o.OrderStatusRecord.Name != OrderStatus.Cancelled.ToString() && o.OrderStatusRecord.Name != OrderStatus.Unapproved.ToString());

            var featuredCampaigns = new FeaturedCampaignViewModel[] { };

            var total =_campaignService.GetAllCampaigns().Count();

            var totalNotApproved = _campaignService.GetAllCampaigns().Where(c => c.IsApproved == false && c.Rejected == false && c.CampaignCulture == cultureUsed).Count();

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
                        Minimum = c.ProductMinimumGoal,
                        Rejected = c.Rejected,
                        Currency = c.CurrencyRecord,
                        FilterCurrencyId = filterCurrencyId
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
                     
                    .OrderBy(c => c.Campaign.Id)              
                    .ToArray();
                campaigns.OrderByDescending(c => c.Id);
            }
          
            return View("Index", new AdminFeaturedCampaignsViewModel { Campaigns = featuredCampaigns,NotApprovedTotal= totalNotApproved,
                                        Currencies = _currencyRepository});
        }

        public ActionResult ChangeVisible(PagerParameters pagerParameters, int id, bool visible)
        {
            var featuredCampaigns = _campaignService.GetAllCampaigns().Where(c => c.IsFeatured && c.CampaignCulture == cultureUsed);

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
            campaign.Rejected = false;
            campaign.WhenApproved = DateTime.UtcNow;

            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            if (!campaign.IsArchived && campaign.BaseCampaignId != null)
            {
                _teeyootMessagingService.SendReLaunchApprovedCampaignMessageToSeller(pathToTemplates, pathToMedia, campaign.Id);
                _teeyootMessagingService.SendReLaunchApprovedCampaignMessageToBuyers(pathToTemplates, pathToMedia, campaign.Id);
            }
            else
            {               
                _teeyootMessagingService.SendLaunchCampaignMessage(pathToTemplates, pathToMedia, campaign.Id);
            }
            
            return RedirectToAction("Index", new { PagerParameters = pagerParameters });
        }

        public ActionResult Reject(PagerParameters pagerParameters, int id)
        {
            var campaign = _campaignService.GetCampaignById(id);
            campaign.Rejected = true;
            campaign.IsApproved = false;
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _teeyootMessagingService.SendRejectedCampaignMessage(pathToTemplates, pathToMedia, campaign.Id);
            return RedirectToAction("Index", new { PagerParameters = pagerParameters });
        }
    }
}