using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCommonSettingsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<CommonSettingsRecord> _commonSettingsRepository;
        private readonly IRepository<CheckoutCampaignRequest> _checkoutRequestRepository;

        public AdminCommonSettingsController(
            IOrchardServices orchardServices,
            IRepository<CommonSettingsRecord> commonSettingsRepository,
            IRepository<CheckoutCampaignRequest> checkoutRequestRepository)
        {
            _orchardServices = orchardServices;
            _commonSettingsRepository = commonSettingsRepository;
            _checkoutRequestRepository = checkoutRequestRepository;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            var commonSettings = _commonSettingsRepository.Table.First();
            var numberOfNotSentEmailCheckoutRequests = _checkoutRequestRepository.Table
                .Count(r => r.EmailSentUtcDate == null);

            var commonSettingsIndexViewModel = new CommonSettingsIndexViewModel
            {
                DoNotAcceptAnyNewCampaigns = commonSettings.DoNotAcceptAnyNewCampaigns,
                NumberOfNotSentEmailCheckoutRequests = numberOfNotSentEmailCheckoutRequests
            };

            return View(commonSettingsIndexViewModel);
        }

        [HttpPost]
        public ActionResult EditDoNotAcceptAnyNewCampaigns(bool doNotAcceptAnyNewCampaigns, bool sendEmails)
        {
            var commonSettings = _commonSettingsRepository.Table.First();
            commonSettings.DoNotAcceptAnyNewCampaigns = doNotAcceptAnyNewCampaigns;
            _commonSettingsRepository.Update(commonSettings);

            _orchardServices.Notifier.Information(T("\"Do not accept any new campaign\" setting changed to {0}.",
                doNotAcceptAnyNewCampaigns ? T("\"Yes\"") : T("\"No\"")));

            return RedirectToAction("Index");
        }
    }
}