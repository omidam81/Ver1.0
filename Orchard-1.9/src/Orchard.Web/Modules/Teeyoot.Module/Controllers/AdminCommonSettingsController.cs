using System;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCommonSettingsController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<CommonSettingsRecord> _commonSettingsRepository;
        private readonly IRepository<CheckoutCampaignRequest> _checkoutRequestRepository;
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        public AdminCommonSettingsController(
            IOrchardServices orchardServices,
            IRepository<CommonSettingsRecord> commonSettingsRepository,
            IRepository<CheckoutCampaignRequest> checkoutRequestRepository,
            ITeeyootMessagingService teeyootMessagingService)
        {
            _orchardServices = orchardServices;
            _commonSettingsRepository = commonSettingsRepository;
            _checkoutRequestRepository = checkoutRequestRepository;
            _teeyootMessagingService = teeyootMessagingService;

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
                NumberOfNotSentEmailCheckoutRequests = numberOfNotSentEmailCheckoutRequests,
                CashOnDeliveryAvailabilityMessage = commonSettings.CashOnDeliveryAvailabilityMessage
            };

            return View(commonSettingsIndexViewModel);
        }

        [HttpPost]
        public ActionResult EditDoNotAcceptAnyNewCampaigns(bool doNotAcceptAnyNewCampaigns, bool sendEmails)
        {
            var commonSettings = _commonSettingsRepository.Table.First();
            commonSettings.DoNotAcceptAnyNewCampaigns = doNotAcceptAnyNewCampaigns;
            _commonSettingsRepository.Update(commonSettings);

            if (sendEmails)
            {
                var checkoutCampaignRequests = _checkoutRequestRepository.Table
                    .Where(r => r.EmailSentUtcDate == null)
                    .ToList();

                _teeyootMessagingService.SendCheckoutRequestEmails(checkoutCampaignRequests);

                var emailSentUtcDate = DateTime.UtcNow;
                checkoutCampaignRequests.ForEach(r =>
                {
                    r.EmailSentUtcDate = emailSentUtcDate;
                    _checkoutRequestRepository.Update(r);
                });
            }

            _orchardServices.Notifier.Information(T("\"Do not accept any new campaign\" setting changed to {0}.",
                doNotAcceptAnyNewCampaigns ? T("\"Yes\"") : T("\"No\"")));

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult EditCashOnDeliveryAvailabilityMessage(string cashOnDeliveryAvailabilityMessage)
        {
            var commonSettings = _commonSettingsRepository.Table.First();
            commonSettings.CashOnDeliveryAvailabilityMessage = cashOnDeliveryAvailabilityMessage;
            _commonSettingsRepository.Update(commonSettings);

            _orchardServices.Notifier.Information(T("\"Cash on delivery availability message\" setting changed."));

            return RedirectToAction("Index");
        }
    }
}