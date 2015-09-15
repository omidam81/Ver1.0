using Mandrill;
using Mandrill.Model;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Roles.Models;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

namespace Teeyoot.Messaging.Services
{
    public class TeeyootMessagingService : ITeeyootMessagingService
    {
        private readonly IRepository<MailChimpSettingsPartRecord> _mailChimpSettingsRepository;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly IContentManager _contentManager;
        private readonly IMessageService _messageService;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IRepository<CampaignRecord> _campaignRepository;
        private readonly IRepository<OrderRecord> _orderRepository;
        private readonly IRepository<LinkOrderCampaignProductRecord> _ocpRepository;
        private readonly IRepository<UserRolesPartRecord> _userRolesPartRepository;
        private readonly IRepository<PayoutRecord> _payoutsRepository;
        private readonly IRepository<PaymentInformationRecord> _payoutInformRepository;
        private readonly IRepository<CampaignProductRecord> _campaignProductRepository;
        private readonly IRepository<BringBackCampaignRecord> _backCampaignRepository;
        private readonly INotifier _notifier;
        private readonly IWorkContextAccessor _wca;
        public Localizer T { get; set; }
        private const string ADMIN_EMAIL = "noreply@teeyoot.com";
        private const string MessageTemplatesPath = "/Modules/Teeyoot.Module/Content/message-templates/";

        public TeeyootMessagingService(IRepository<MailChimpSettingsPartRecord> mailChimpSettingsRepository, IContentManager contentManager, IRepository<CampaignRecord> campaignRepository,
            IMailChimpSettingsService settingsService,
            IMessageService messageService,
             INotifier notifier,
            IRepository<OrderRecord> orderRepository,
            IRepository<LinkOrderCampaignProductRecord> ocpRepository,
            IRepository<UserRolesPartRecord> userRolesPartRepository,
            IRepository<PayoutRecord> payoutsRepository,
            IRepository<PaymentInformationRecord> payoutInformRepository,
            IWorkContextAccessor wca,
            IRepository<CampaignProductRecord> campaignProductRepository,
            IRepository<CurrencyRecord> currencyRepository,
            IRepository<BringBackCampaignRecord> backCampaignRepository)
        {
            _mailChimpSettingsRepository = mailChimpSettingsRepository;
            _contentManager = contentManager;
            _messageService = messageService;
            _settingsService = settingsService;
            _notifier = notifier;
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _currencyRepository = currencyRepository;
            _campaignRepository = campaignRepository;
            _userRolesPartRepository = userRolesPartRepository;
            _payoutsRepository = payoutsRepository;
            _payoutInformRepository = payoutInformRepository;
            _wca = wca;
            _campaignProductRepository = campaignProductRepository;
            _backCampaignRepository = backCampaignRepository;
        }

        public void SendCheckoutRequestEmails(IEnumerable<CheckoutCampaignRequest> checkoutCampaignRequests)
        {
            var culture = _wca.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            var pathToTemplates = HttpContext.Current.Server.MapPath(MessageTemplatesPath);
            var record = _settingsService.GetSettingByCulture(cultureUsed).List().First();
            var api = new MandrillApi(record.ApiKey);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            var mandrillMessage = new MandrillMessage
            {
                MergeLanguage = MandrillMessageMergeLanguage.Handlebars,
                FromEmail = "noreply@teeyoot.com",
                FromName = "Teeyoot",
                Subject = "Now you can create a campaign!"
            };

            List<string> checkoutEmails = checkoutCampaignRequests.Select(r =>r.Email).ToList();
            List<string> resultEmails = new List<string>();
            var noDupes = new HashSet<string>(checkoutEmails);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Buyer"));
            }

            mandrillMessage.To = emails;
            var text = File.ReadAllText(pathToTemplates + "make_the_order_buyer-" + cultureUsed + ".html");
            mandrillMessage.Html = text;
            var res = SendTmplMessage(api, mandrillMessage);
        }

        public void SendExpiredCampaignMessageToSeller(int campaignId, bool isSuccesfull)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            if (isSuccesfull)
            {
                mandrillMessage.Subject = "We are printing one of your designs!";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "campaign-is-printing-seller-template-" + campaign.CampaignCulture + ".html");
            }
            else
            {
                if (campaign.ProductCountSold < campaign.ProductMinimumGoal)
                {
                    mandrillMessage.Subject = "Your campaign didn't reach the minimum";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "not-reach-goal-seller-template-" + campaign.CampaignCulture + ".html");
                }
                else
                {
                    mandrillMessage.Subject = "Your campaign has ended, you did just fine!";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "not-reach-goal-met-minimum-seller-template-" + campaign.CampaignCulture + ".html");
                }
            }

            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            SendTmplMessage(api, mandrillMessage);
        }


        public void SendExpiredCampaignMessageToAdmin(int campaignId, bool isSuccesfull)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            if (isSuccesfull)
            {
                mandrillMessage.Subject = "A campaign just ended - target";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "expired-campaign-successfull-admin-template-" + campaign.CampaignCulture + ".html");
            }
            else
            {
                if (campaign.ProductCountSold < campaign.ProductMinimumGoal)
                {
                    mandrillMessage.Subject = "A campaign just ended - no success";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "expired-campaign-notSuccessfull-admin-template-" + campaign.CampaignCulture + ".html");
                }
                else
                {
                    mandrillMessage.Subject = "A campaign just ended - minimum";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "expired-campaign-met-minimum-admin-template-" + campaign.CampaignCulture + ".html");
                }
            }


            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillCampaignMergeVars(mandrillMessage, campaignId, user.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            
            SendTmplMessage(api, mandrillMessage);
        }






        public void SendExpiredCampaignMessageToBuyers(int campaignId, bool isSuccesfull)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            if (isSuccesfull)
            {
                mandrillMessage.Subject = "Yay! we are printing them";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "order-is-printing-buyer-template-" + campaign.CampaignCulture + ".html");
            }
            else
            {
                if (campaign.ProductCountSold < campaign.ProductMinimumGoal)
                {
                    mandrillMessage.Subject = "Oops! We're not printing this";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "not-reach-goal-buyer-template-" + campaign.CampaignCulture + ".html");
                }
                else
                {
                    mandrillMessage.Subject = "Yay! we are printing them";
                    mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "order-is-printing-buyer-template-" + campaign.CampaignCulture + ".html");
                }
            }

            List<LinkOrderCampaignProductRecord> ordersList = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive).ToList();           
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            List<string> emailsList = new List<string>();
            foreach (var item in ordersList)
            {
                if (item.OrderRecord.Email != null)
                {
                    emailsList.Add(item.OrderRecord.Email);
                    FillUserMergeVars(mandrillMessage, item.OrderRecord);
                    FillProductsMergeVars(mandrillMessage, item.OrderRecord.Products, pathToMedia, item.OrderRecord.Email, item.OrderRecord.OrderPublicId);
                    FillCampaignMergeVars(mandrillMessage, campaignId, item.OrderRecord.Email, pathToMedia, pathToTemplates);
                }
            }
            List<string> resultEmails = new List<string>();
            var noDupes = new HashSet<string>(emailsList);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Buyer"));
            }
            mandrillMessage.To = emails; 
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendCampaignMetMinimumMessageToBuyers(int campaignId)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "We're definitely printing this!";
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "definitely-go-to-print-buyer-template-" + campaign.CampaignCulture + ".html");
            List<LinkOrderCampaignProductRecord> ordersList = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive).ToList();
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            List<string> emailsList = new List<string>();
            foreach (var item in ordersList)
            {

                if (item.OrderRecord.Email != null)
                {
                    emailsList.Add(item.OrderRecord.Email);
                    FillUserMergeVars(mandrillMessage, item.OrderRecord);
                    FillProductsMergeVars(mandrillMessage, item.OrderRecord.Products, pathToMedia, item.OrderRecord.Email, item.OrderRecord.OrderPublicId);
                    FillCampaignMergeVars(mandrillMessage, campaignId, item.OrderRecord.Email, pathToMedia, pathToTemplates);
                }
            }
            List<string> resultEmails = new List<string>();
            var noDupes = new HashSet<string>(emailsList);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Buyer"));
            }
            mandrillMessage.To = emails;
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendCampaignMetMinimumMessageToSeller(int campaignId)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Your campaign has hit the minimum!";
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "definitely-go-to-print-seller-template-" + campaign.CampaignCulture + ".html");
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendAllOrderDeliveredMessageToSeller(int campaignId)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "All orders from your campaign was delivered!";
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "all-orders-delivered-seller-template-" + campaign.CampaignCulture + ".html");
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendLaunchCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Yay! your new campaign has been approved";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "launch-template-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }
        public void SendReLaunchApprovedCampaignMessageToSeller(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);

            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Re-enable Campaign on Teeyoot";

            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };

            FillRelaunchCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);

            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "relaunch-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);
        }
        
        public void SendReLaunchApprovedCampaignMessageToBuyers(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);

            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Re-enable Campaign on Teeyoot";
           
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            List<string> emailsList = new List<string>();
            List<string> resultEmails = new List<string>();
            var buyers = _backCampaignRepository.Table.Where(c => c.CampaignRecord.Id == campaign.BaseCampaignId).ToList();

            foreach (var buyer in buyers)
            {
                emailsList.Add(buyer.Email);
                FillRelaunchCampaignMergeVars(mandrillMessage, campaignId, buyer.Email, pathToMedia, pathToTemplates);
            }
            
            var noDupes = new HashSet<string>(emailsList);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Buyer"));
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "relaunch-buyer-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);
            
        }

        public void SendReLaunchCampaignMessageToAdmin( int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);

            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");

            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Re-enable Campaign on Teeyoot";

            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);

            List<string> emailsList = new List<string>();
            List<string> resultEmails = new List<string>();
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emailsList.Add(user.Email);
                FillRelaunchCampaignMergeVars(mandrillMessage, campaignId, user.Email, pathToMedia, pathToTemplates);
            }

            var noDupes = new HashSet<string>(emailsList);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Admin"));
            }
            mandrillMessage.To = emails;

            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "relaunch-to-admin-seller-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);
        }
        public void SendReLaunchCampaignMessageToSeller(int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);

            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");

            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Re-enable Campaign on Teeyoot";

            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };

            FillRelaunchCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "relaunch-to-admin-seller-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);
        }


        public void SendTermsAndConditionsMessageToSeller()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaigns = _campaignRepository.Table.Where(camp => camp.WhenApproved < DateTime.UtcNow.AddDays(-1) && camp.WhenApproved > DateTime.UtcNow.AddDays(-3));
            MailChimpSettingsPart record = null;
            if (campaigns != null || campaigns.Count() != 0)
            {
                record = _settingsService.GetSettingByCulture(campaigns.First().CampaignCulture).List().First();
            }
            else
            {
                record = _settingsService.GetAllSettings().List().FirstOrDefault();
            }
            var api = new MandrillApi(record.ApiKey);
            foreach (var campaign in campaigns)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.FromName = "Teeyoot";
                mandrillMessage.Subject = "Promote your campaign";
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
                mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")};
                FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "terms-conditions-template-" + campaign.CampaignCulture + ".html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendCampaignFinished1DayMessageToSeller()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaigns = _campaignRepository.Table.Where(camp => camp.EndDate < DateTime.UtcNow.AddDays(-1) && camp.EndDate > DateTime.UtcNow.AddDays(-3) && camp.IsApproved);
            MailChimpSettingsPart record = null;
            if (campaigns != null || campaigns.Count() != 0)
            {
                record = _settingsService.GetSettingByCulture(campaigns.First().CampaignCulture).List().First();
            }
            else
            {
                record = _settingsService.GetAllSettings().List().FirstOrDefault();
            }
            var api = new MandrillApi(record.ApiKey);
            foreach (var campaign in campaigns)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.FromName = "Teeyoot";
                mandrillMessage.Subject = "Campaing is finished!";
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
                mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")};
                FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "campaign-is-finished-template-" + campaign.CampaignCulture + ".html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendOrderShipped3DaysToBuyer()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
           
            var orders = _orderRepository.Table.Where(order => order.WhenSentOut < DateTime.UtcNow.AddDays(-1) && order.WhenSentOut > DateTime.UtcNow.AddDays(-3));
            MailChimpSettingsPart record = null;
            if (orders != null || orders.Count() != 0)
            {
                record = _settingsService.GetSettingByCulture(orders.First().CurrencyRecord.CurrencyCulture).List().First();
            }
            else
            {
                record = _settingsService.GetAllSettings().List().FirstOrDefault();
            }
            var api = new MandrillApi(record.ApiKey);
            foreach (var order in orders)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.FromName = "Teeyoot";
                mandrillMessage.Subject = "Your order should be around the corner";
                List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                emails.Add(new MandrillMailAddress(order.Email, "Buyer"));
                mandrillMessage.To = emails;
                FillUserMergeVars(mandrillMessage, order, order.Email);
                FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "shipped-order-3day-after-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendRejectedCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Sorry, we couldn't approve your campaign";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "reject-template-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendNewCampaignAdminMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Yay! new campaign";
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillCampaignMergeVars(mandrillMessage, campaignId, user.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-campaign-admin-template-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendCompletedPayoutMessage(string pathToTemplates, string pathToMedia, PayoutRecord payout)
        {
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == payout.UserId);
            var payoutInf = _payoutInformRepository.Table.Where(inf => inf.TranzactionId == payout.Id).FirstOrDefault();
            var currency = _currencyRepository.Get(payout.Currency_Id).Code;
            var culture = _currencyRepository.Get(payout.Currency_Id).CurrencyCulture;
            var record = _settingsService.GetSettingByCulture(culture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "We have paid you. Definitely!";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(seller.Email, "Seller"));
            FillPayoutRequestMergeVars(mandrillMessage, seller.Email, seller.Id, payoutInf.AccountNumber.ToString(), payoutInf.BankName.ToString(), payoutInf.AccountHolderName.ToString(), payoutInf.ContactNumber.ToString(), "", payout.Amount, currency);
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "withdraw-completed-template-" + _currencyRepository.Get(payout.Currency_Id).CurrencyCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendChangedCampaignStatusMessage(int campaignId, string campaignStatus)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            switch (campaignStatus)
            {
                case "Unpaid":
                    {
                        mandrillMessage.Subject = "We haven't paid you yet!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "unpaid-campaign-template-" + campaign.CampaignCulture + ".html");
                        break;
                    };
                case "Paid":
                    {
                        mandrillMessage.Subject = "We have paid you!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "paid-campaign-template-" + campaign.CampaignCulture + ".html");
                        break;
                    };
                //case "PartiallyPaid":
                //    {
                //        mandrillMessage.Subject = "We have partially paid you!";
                //        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "partially-paid-campaign-template.html");
                //        break;
                //    };
            }
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendSellerMessage(int messageId, string pathToMedia, string pathToTemplates)
        {
            var message = _messageService.GetMessage(messageId);
            List<LinkOrderCampaignProductRecord> ordersList = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == message.CampaignId && p.OrderRecord.IsActive).ToList();
            var culture = ordersList.First().CampaignProductRecord.CurrencyRecord.CurrencyCulture;
            var record = _settingsService.GetSettingByCulture(culture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = message.Sender;
            mandrillMessage.Subject = message.Subject;
            mandrillMessage.FromName = "Teeyoot";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            List<string> emailsList = new List<string>();
            foreach (var item in ordersList)
            {
                if (item.OrderRecord.Email != null)
                {
                    emailsList.Add(item.OrderRecord.Email);
                    FillUserMergeVars(mandrillMessage, item.OrderRecord);
                    FillSellerToBuyersProductsMergeVars(mandrillMessage, item.OrderRecord.Products, pathToMedia, item.OrderRecord.Email, item.OrderRecord.OrderPublicId);
                    FillCampaignMergeVars(mandrillMessage, message.CampaignId, item.OrderRecord.Email, pathToMedia, pathToTemplates);
                }
            }
            List<string> resultEmails = new List<string>();
            var noDupes = new HashSet<string>(emailsList);
            resultEmails.Clear();
            resultEmails.AddRange(noDupes);
            foreach (var item in resultEmails)
            {
                emails.Add(new MandrillMailAddress(item, "Buyer"));
            }
            mandrillMessage.To = emails;
            string text = System.IO.File.ReadAllText(pathToTemplates + "seller-template-" + ordersList.First().OrderRecord.CurrencyRecord.CurrencyCulture + ".html").Replace("{{Text}}", message.Text);
            mandrillMessage.Html = text;
            message.IsApprowed = true;
            var res = SendTmplMessage(api, mandrillMessage);
        }

        public void SendNewOrderMessageToAdmin(int orderId, string pathToMedia, string pathToTemplates)
        {
            var order = _orderRepository.Get(orderId);
            //string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            //string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetSettingByCulture(order.CurrencyRecord.CurrencyCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "New order";
            mandrillMessage.FromName = "Teeyoot";
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillUserMergeVars(mandrillMessage, order, user.Email);
                FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, user.Email, order.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, user.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-order-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendNewOrderMessageToBuyer(int orderId, string pathToMedia, string pathToTemplates)
        {
            var order = _orderRepository.Get(orderId);
            var record = _settingsService.GetSettingByCulture(order.CurrencyRecord.CurrencyCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Thanks for your purchase!";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                emails.Add(new MandrillMailAddress(order.Email, "Buyer"));
                FillUserMergeVars(mandrillMessage, order, order.Email);
                FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-order-buyer-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendEditedCampaignMessageToSeller(int campaignId, string pathToMedia, string pathToTemplates)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetSettingByCulture(campaign.CampaignCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "We edited your campaign";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            emails.Add(new MandrillMailAddress(seller.Email, "Seller"));
            List<CampaignProductRecord> orderedProducts = _campaignProductRepository.Table.Where(prod => prod.CampaignRecord_Id == campaign.Id && prod.WhenDeleted == null).ToList();
            FillCampaignProductsMergeVars(mandrillMessage, orderedProducts, pathToMedia, seller.Email);
            FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
            FillAdditionalCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "edited-campaign-template-" + campaign.CampaignCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendPayoutRequestMessageToAdmin(int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin)
        {
            var culture = _wca.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            var teeUser = _contentManager.Get<TeeyootUserPart>(userIds.First(), VersionOptions.Latest);
            var record = _settingsService.GetSettingByCulture(cultureUsed).List().First();
            if (teeUser !=null)
            {
                record = _settingsService.GetSettingByCulture(teeUser.TeeyootUserCulture).List().First();
                cultureUsed = teeUser.TeeyootUserCulture;
            }         
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Someone wants to withdraw";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillPayoutRequestMergeVars(mandrillMessage, user.Email, userId, accountNumber, bankName, accHoldName, contNum, messAdmin, 0.00, "");
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "withdraw-template-" + cultureUsed + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendPayoutRequestMessageToSeller(int userId, string accountNumber, string bankName, string accHoldName, string contNum)
        {
            var culture = _wca.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var use = _contentManager.Get<TeeyootUserPart>(userId, VersionOptions.Latest);
            if (use != null)
            {
                cultureUsed = use.TeeyootUserCulture;
            }
            var record = _settingsService.GetSettingByCulture(cultureUsed).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "We have received your payout request";
            var user = _contentManager.Get<UserPart>(userId, VersionOptions.Latest);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(user.Email, "Seller"));
            FillPayoutRequestMergeVars(mandrillMessage, user.Email, userId, accountNumber, bankName, accHoldName, contNum, "", 0.00, "");
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "withdraw-seller-template-" + cultureUsed + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendOrderStatusMessage(string pathToTemplates, string pathToMedia, int orderId, string orderStatus)
        {
            var order = _orderRepository.Get(orderId);
            var record = _settingsService.GetSettingByCulture(order.CurrencyRecord.CurrencyCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            switch (orderStatus)
            {
                case "Approved":
                    {
                        mandrillMessage.Subject = "Thanks for your purchase!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "place-order-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                        FillUserMergeVars(mandrillMessage, order);
                        FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                        FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                        mandrillMessage.To = new List<MandrillMailAddress>(){
                         new MandrillMailAddress(order.Email, "Buyer")
                                                         };
                        SendTmplMessage(api, mandrillMessage);
                        break;
                    };
                case "Printing":
                    {
                        mandrillMessage.Subject = "Yay! we are printing them";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "order-is-printing-buyer-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                        FillUserMergeVars(mandrillMessage, order);
                        FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                        FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                        mandrillMessage.To = new List<MandrillMailAddress>(){
                                             new MandrillMailAddress(order.Email, "Buyer")
                                                            };
                        SendTmplMessage(api, mandrillMessage);
                        break;
                    };
                case "Shipped":
                    {
                        mandrillMessage.Subject = "Your order is on its way!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "shipped-order-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                        FillUserMergeVars(mandrillMessage, order);
                        FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                        FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                        mandrillMessage.To = new List<MandrillMailAddress>(){
                                                                 new MandrillMailAddress(order.Email, "Buyer")
                                                                 };
                        SendTmplMessage(api, mandrillMessage);
                        break;
                    };
                case "Delivered":
                    {
                        mandrillMessage.Subject = "We have delivered your order!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "delivered-order-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                        FillUserMergeVars(mandrillMessage, order);
                        FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                        FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                        mandrillMessage.To = new List<MandrillMailAddress>(){
                                                         new MandrillMailAddress(order.Email, "Buyer")
                                                     };
                        SendTmplMessage(api, mandrillMessage);
                        break;
                    };
                case "Cancelled":
                    {
                        mandrillMessage.Subject = "Order was cancelled";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "cancelled-order-template-" + order.CurrencyRecord.CurrencyCulture + ".html");
                        FillUserMergeVars(mandrillMessage, order);
                        FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                        FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                        mandrillMessage.To = new List<MandrillMailAddress>(){
                                                     new MandrillMailAddress(order.Email, "Buyer")
                                            };
                        SendTmplMessage(api, mandrillMessage);
                        break;
                    };

            }

        }

        public void SendRecoverOrderMessage(string pathToTemplates, IList<OrderRecord> orders, string email)
        {

            var record = _settingsService.GetSettingByCulture(orders.First().CurrencyRecord.CurrencyCulture).List().First();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.FromName = "Teeyoot";
            mandrillMessage.Subject = "Your current orders";           
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(email, "Buyer")
            };
            FillOrdersMergeVars(mandrillMessage, orders, email, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "recover_orders_for_buyer-" + orders.First().CurrencyRecord.CurrencyCulture + ".html");
            SendTmplMessage(api, mandrillMessage);

        }

        private void FillOrdersMergeVars(MandrillMessage message, IList<OrderRecord> orders, string email, string orderPublicId)
        {
            List<Dictionary<string, object>> ordersList = new List<Dictionary<string, object>>();
            foreach (var item in orders)
            {
                int index = orders.IndexOf(item);
                int quantity = item.Products.Sum(m => m.Count);
                var campaign = _campaignRepository.Get(item.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id);
                
                ordersList.Add(new Dictionary<string, object>{                 
                        {"id", item.OrderPublicId},
                        {"quantity",  quantity},
                        {"campaign", campaign.Title},
                        {"created",  item.Created.ToLocalTime().ToString()}
                     });

            }
            var arr = ordersList.ToArray();
            message.AddRcptMergeVars(email, "ORDERS", ordersList.ToArray());           
        }

        private void FillCampaignMergeVars(MandrillMessage message, int campaignId, string email, string pathToMedia, string pathToTemplates)
        {
            var baseUrl = "";
            string remaining = "";
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            }
            else
            {
                baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            }
            string side = "";
            var campaign = _campaignRepository.Get(campaignId);

             if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days > 0)
            {
           remaining = campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days + " days";
             }
             else if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days <= -1)
             { 
            remaining = Math.Abs(campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days) + "days ago";
                 }
                 else
             {
             if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours > 0)
            { 
                remaining = campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours + "hours";
            }
            else
            {
                remaining = Math.Abs(campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours) + "hours ago";
            }
        }


            if (campaign.BackSideByDefault)
            {
                side = "back";
            }
            else
            {
                side = "front";
            }
            message.AddRcptMergeVars(email, "CampaignTitle", campaign.Title);
            message.AddRcptMergeVars(email, "Campaignremaining", remaining);
            message.AddRcptMergeVars(email, "Url", baseUrl);
            message.AddRcptMergeVars(email, "CampaignAlias", campaign.Alias);
            message.AddRcptMergeVars(email, "ReservedCount", campaign.ProductCountSold.ToString());
            message.AddRcptMergeVars(email, "Goal", campaign.ProductCountGoal.ToString());
            message.AddRcptMergeVars(email, "SellerEmail", _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId).Email);
            message.AddRcptMergeVars(email, "CampaignPreviewUrl", baseUrl + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products.First(p => p.WhenDeleted == null).Id + "/normal/" + side + ".png");
            message.AddRcptMergeVars(email, "VideoPreviewUrl", baseUrl + "/Media/Default/images/video_thumbnail_521x315.jpg/");

        }

        private void FillRelaunchCampaignMergeVars(MandrillMessage message, int campaignId, string email, string pathToMedia, string pathToTemplates)
        {
            var baseUrl = "";
            string remaining = "";
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            }
            else
            {
                baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            }
            string side = "";
            var campaign = _campaignRepository.Get(campaignId);

            if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days > 0)
            {
                remaining = campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days + " days";
            }
            else if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days <= -1)
            {
                remaining = Math.Abs(campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Days) + "days ago";
            }
            else
            {
                if (campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours > 0)
                {
                    remaining = campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours + "hours";
                }
                else
                {
                    remaining = Math.Abs(campaign.EndDate.ToLocalTime().Subtract(DateTime.UtcNow).Hours) + "hours ago";
                }
            }


            if (campaign.BackSideByDefault)
            {
                side = "back";
            }
            else
            {
                side = "front";
            }
            message.AddRcptMergeVars(email, "CampaignTitle", campaign.Title);
            message.AddRcptMergeVars(email, "Url", baseUrl);
            message.AddRcptMergeVars(email, "Campaignremaining", remaining);
            message.AddRcptMergeVars(email, "CampaignEndDate", campaign.EndDate.ToLocalTime().ToShortDateString());
            message.AddRcptMergeVars(email, "CampaignAlias", campaign.Alias);
            message.AddRcptMergeVars(email, "CampaignPreviewUrl", baseUrl + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products.First(p => p.WhenDeleted == null).Id + "/normal/" + side + ".png");
        }

        private void FillAdditionalCampaignMergeVars(MandrillMessage message, int campaignId, string email, string pathToMedia, string pathToTemplates)
        {
            var campaign = _campaignRepository.Get(campaignId);
            message.AddRcptMergeVars(email, "Description", campaign.Description);
            message.AddRcptMergeVars(email, "Expiration", campaign.EndDate.ToShortDateString());
            message.AddRcptMergeVars(email, "Profit", campaign.CampaignProfit);

        }

        private void FillUserMergeVars(MandrillMessage message, OrderRecord record)
        {

            message.AddRcptMergeVars(record.Email, "FNAME", record.FirstName);
            message.AddRcptMergeVars(record.Email, "LNAME", record.LastName);
            message.AddRcptMergeVars(record.Email, "CITY", record.City);
            message.AddRcptMergeVars(record.Email, "CLIENT_EMAIL", record.Email);
            message.AddRcptMergeVars(record.Email, "STATE", record.State);
            message.AddRcptMergeVars(record.Email, "STREET_ADDRESS", record.StreetAddress);
            message.AddRcptMergeVars(record.Email, "COUNTRY", record.Country);

            double totalPrice;

            if (record.TotalPriceWithPromo > 0.0)
            {
                totalPrice = record.TotalPriceWithPromo + record.Delivery;
                message.AddRcptMergeVars(record.Email, "TOTALPRICE", totalPrice.ToString("F", CultureInfo.InvariantCulture));
            }
            else
            {
                totalPrice = record.TotalPrice + record.Delivery;
                message.AddRcptMergeVars(record.Email, "TOTALPRICE", totalPrice.ToString("F", CultureInfo.InvariantCulture));
            }
        }

        private void FillPayoutRequestMergeVars(MandrillMessage message, string adminEmail, int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin, double amount, string currencyCode)
        {

            var baseUrl = "";
            
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            }
            else
            {
                baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            }
            var requester = _contentManager.Get<TeeyootUserPart>(userId, VersionOptions.Latest);

            message.AddRcptMergeVars(adminEmail, "Requester_Name", requester.PublicName);
            message.AddRcptMergeVars(adminEmail, "AccountNumber", accountNumber);
            message.AddRcptMergeVars(adminEmail, "BankName", bankName);
            message.AddRcptMergeVars(adminEmail, "AccHolderName", accHoldName);
            message.AddRcptMergeVars(adminEmail, "ContactNumber", contNum);
            message.AddRcptMergeVars(adminEmail, "Text", messAdmin);
            message.AddRcptMergeVars(adminEmail, "Amount", amount.ToString("F", CultureInfo.InvariantCulture));
            message.AddRcptMergeVars(adminEmail, "Currency", currencyCode);
            message.AddRcptMergeVars(adminEmail, "Url", baseUrl);

         

        }

        private void FillUserMergeVars(MandrillMessage message, OrderRecord record, string adminEmail)
        {

            message.AddRcptMergeVars(adminEmail, "FNAME", record.FirstName);
            message.AddRcptMergeVars(adminEmail, "LNAME", record.LastName);
            message.AddRcptMergeVars(adminEmail, "CITY", record.City);
            message.AddRcptMergeVars(adminEmail, "CLIENT_EMAIL", record.Email);
            message.AddRcptMergeVars(adminEmail, "STATE", record.State);
            message.AddRcptMergeVars(adminEmail, "PHONE", record.PhoneNumber);
            message.AddRcptMergeVars(adminEmail, "STREET_ADDRESS", record.StreetAddress);
            message.AddRcptMergeVars(adminEmail, "COUNTRY", record.Country);

            double totalPrice;

            if (record.TotalPriceWithPromo > 0.0)
            {
                totalPrice = record.TotalPriceWithPromo + record.Delivery;
                message.AddRcptMergeVars(adminEmail, "TOTALPRICE", totalPrice.ToString("F", CultureInfo.InvariantCulture));
            }
            else
            {
                totalPrice = record.TotalPrice + record.Delivery;
                message.AddRcptMergeVars(adminEmail, "TOTALPRICE", totalPrice.ToString("F", CultureInfo.InvariantCulture));
            }

        }

        private void FillProductsMergeVars(MandrillMessage message, IList<LinkOrderCampaignProductRecord> orderedProducts, string pathToMedia, string email, string orderPublicId)
        {
            string baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();
            foreach (var item in orderedProducts)
            {
               
                string side = "";
                var campaign = _campaignRepository.Get(item.CampaignProductRecord.CampaignRecord_Id);
                if (campaign.BackSideByDefault)
                {
                    side = "back";
                }
                else
                {
                    side = "front";
                }
                int index = orderedProducts.IndexOf(item);
                int idSize = item.ProductSizeRecord.Id;
                float costSize = item.CampaignProductRecord.ProductRecord.SizesAvailable.Where(c => c.ProductSizeRecord.Id == idSize).First().SizeCost;
                float price = (float)item.CampaignProductRecord.Price + costSize;
                string prodColor = "";
                if (item.ProductColorRecord != null)
                {
                    if (item.CampaignProductRecord.ProductColorRecord.Id == item.ProductColorRecord.Id)
                    {
                        prodColor = item.CampaignProductRecord.Id.ToString();
                    }
                    else
                    {
                        prodColor = item.CampaignProductRecord.Id + "_" + item.ProductColorRecord.Id.ToString();
                    }
                }
                else
                {
                    prodColor = item.CampaignProductRecord.Id.ToString();
                }
                products.Add(new Dictionary<string, object>{                 
                        {"quantity", item.Count},
                        {"name",  item.CampaignProductRecord.ProductRecord.Name},
                        {"description",  item.CampaignProductRecord.ProductRecord.Details},
                        {"price", price},
                        {"size", item.ProductSizeRecord.SizeCodeRecord.Name},
                        {"currency", item.OrderRecord.CurrencyRecord.Code},
                        {"total_price", (price* item.Count).ToString("F", CultureInfo.InvariantCulture)},
                        {"preview_url", baseUrl + "/Media/campaigns/" + item.CampaignProductRecord.CampaignRecord_Id + "/" + prodColor + "/normal/"+side+".png"}
                     });

            }
            var arr = products.ToArray();
            message.AddRcptMergeVars(email, "PRODUCTS", products.ToArray());
            message.AddRcptMergeVars(email, "orderPublicId", orderPublicId);
        }

        private void FillCampaignProductsMergeVars(MandrillMessage message, IList<CampaignProductRecord> campaignProducts, string pathToMedia, string email)
        {
            string baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();
            foreach (var item in campaignProducts)
            {
                string side = "";
                var campaign = _campaignRepository.Get(item.CampaignRecord_Id);
                if (campaign.BackSideByDefault)
                {
                    side = "back";
                }
                else
                {
                    side = "front";
                }
                products.Add(new Dictionary<string, object>{                 
                        {"name",  item.ProductRecord.Name},
                        {"price", item.Price},
                        {"currency", item.CurrencyRecord.Code},
                        {"preview_url", baseUrl + "/Media/campaigns/" + item.CampaignRecord_Id + "/" + item.Id + "/normal/"+side+".png"}
                     });

            }
            var arr = products.ToArray();
            message.AddRcptMergeVars(email, "PRODUCTS", products.ToArray());
        }

        private void FillSellerToBuyersProductsMergeVars(MandrillMessage message, IList<LinkOrderCampaignProductRecord> orderedProducts, string pathToMedia, string email, string orderPublicId)
        {
            string products = "";
            var i = 0;
            //List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();
            foreach (var item in orderedProducts)
            {

                int index = orderedProducts.IndexOf(item);
                int idSize = item.ProductSizeRecord.Id;
                float costSize = item.CampaignProductRecord.ProductRecord.SizesAvailable.Where(c => c.ProductSizeRecord.Id == idSize).First().SizeCost;
                float price = (float)item.CampaignProductRecord.Price + costSize;
                if (i > 0)
                {
                    products += item.Count.ToString() + " x " + item.ProductSizeRecord.SizeCodeRecord.Name + " " + item.CampaignProductRecord.ProductRecord.Name + ", " + Environment.NewLine;
                }
                else
                {
                    products += item.Count.ToString() + " x " + item.ProductSizeRecord.SizeCodeRecord.Name + " " + item.CampaignProductRecord.ProductRecord.Name + Environment.NewLine;
                }
                i++;
              

            }
            message.AddRcptMergeVars(email, "PRODUCTS", products);
            message.AddRcptMergeVars(email, "orderPublicId", orderPublicId);
        }


        private MandrillMessage InitMandrillMessage(OrderRecord order)
        {
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.Subject = "Your order";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(order.Email));
            mandrillMessage.To = emails;
            return mandrillMessage;
        }



        private string SendTmplMessage(MandrillApi mAPI, Mandrill.Model.MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }

    }
}
