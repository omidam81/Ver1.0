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
        private readonly IRepository<LinkOrderCampaignProductRecord> _linkOrderCampaignProductRepository;
        private readonly IWorkContextAccessor _wca;

        public DashboardController(ICampaignService campaignService, 
                                   IMailChimpSettingsService settingsService, 
                                   IRepository<LinkOrderCampaignProductRecord> linkOrderCampaignProductRepository,
                                   IOrderService orderService,
                                   IWorkContextAccessor wca)
        {
            _campaignService = campaignService;
            _orderService = orderService;
            _wca = wca;
            this._settingsService = settingsService;
            this._linkOrderCampaignProductRepository = linkOrderCampaignProductRepository;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
    }
}