using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using System.Web.Mvc;
using Teeyoot.Messaging.Services;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Dashboard.Controllers
{
    [Themed]
    public partial class DashboardController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IOrderService _orderService;
        private readonly IWorkContextAccessor _wca;
        private readonly IStoreService _storeService;
        private readonly IimageHelper _imageHelper;
        private readonly IMembershipService _membershipService;
        private readonly IContentManager _contentManager;
        private readonly IPayoutService _payoutService;
        private readonly IPromotionService _promotionService;
        private IOrchardServices Services { get; set; }

        public DashboardController(ICampaignService campaignService, 
                                   IMailChimpSettingsService settingsService, 
                                   IOrderService orderService,
                                   IWorkContextAccessor wca,
                                   IStoreService storeService,
                                   IimageHelper imageHelper,
                                   IMembershipService membershipService,
                                   IPayoutService payoutService,
                                   IOrchardServices services,
                                   IContentManager contentManager,
                                   IPromotionService promotionService)
        {
            _campaignService = campaignService;
            _orderService = orderService;
            _wca = wca;
            this._settingsService = settingsService;
            this._orderService = orderService;
            _storeService = storeService;
            _imageHelper = imageHelper;
            _membershipService = membershipService;
            _contentManager = contentManager;
            _payoutService = payoutService;
            _promotionService = promotionService;
            Services = services;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
    }
}