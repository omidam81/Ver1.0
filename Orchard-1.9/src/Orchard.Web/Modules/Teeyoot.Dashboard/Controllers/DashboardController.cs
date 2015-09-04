using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using System.Web.Mvc;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Dashboard.Services;
using Orchard.Localization;
using Orchard.UI.Notify;

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
        private readonly ICampaignCategoriesService _campaignCategoryService;
        private readonly IPaymentInformationService _paymentInfService;
        private readonly IMessageService _messageService;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly ITShirtCostService _tshirtService;
        private readonly IProductService _productService;
        private readonly INotifier _notifier;
        private IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private string culture = string.Empty;
        private string cultureUsed = string.Empty;

        public Localizer T { get; set; }

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
                                   IPromotionService promotionService,
                                   IPaymentInformationService paymentInfService,
                                   ICampaignCategoriesService campaignCategoryService,
                                   IRepository<CurrencyRecord> currencyRepository,
                                   IMessageService messageService,
                                   ITShirtCostService tshirtService,
                                   IProductService productService,
                                   INotifier notifier,
                                   ITeeyootMessagingService teeyootMessagingService,
                                   IWorkContextAccessor workContextAccessor
                                    )
        {
            _campaignService = campaignService;
            _currencyRepository = currencyRepository;
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
            _campaignCategoryService = campaignCategoryService;
            _paymentInfService = paymentInfService;
            _messageService = messageService;
            _tshirtService = tshirtService;
            _productService = productService;
            _notifier = notifier;
            _teeyootMessagingService = teeyootMessagingService;
            Services = services;

            Logger = NullLogger.Instance;

            _workContextAccessor = workContextAccessor;
            culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public ILogger Logger { get; set; }
    }
}