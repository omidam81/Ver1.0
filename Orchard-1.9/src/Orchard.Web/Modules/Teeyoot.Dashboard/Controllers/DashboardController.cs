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
        private readonly IRepository<LinkOrderCampaignProductRecord> _linkOrderCampaignProductRepository;

        public DashboardController(ICampaignService campaignService, 
                                   IMailChimpSettingsService settingsService, 
                                   IRepository<LinkOrderCampaignProductRecord> linkOrderCampaignProductRepository)
        {
            _campaignService = campaignService;
            this._settingsService = settingsService;
            this._linkOrderCampaignProductRepository = linkOrderCampaignProductRepository;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }
    }
}