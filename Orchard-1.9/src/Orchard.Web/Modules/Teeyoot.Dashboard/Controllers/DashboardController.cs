using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Themes;
using System.Web.Mvc;
using Teeyoot.Messaging.Services;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

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

        public DashboardController(ICampaignService campaignService, 
                                   IMailChimpSettingsService settingsService, 
                                   IOrderService orderService,
                                   IWorkContextAccessor wca,
                                   IStoreService storeService)
        {
            _campaignService = campaignService;
            _orderService = orderService;
            _wca = wca;
            this._settingsService = settingsService;
            this._orderService = orderService;
            _storeService = storeService;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
    }
}