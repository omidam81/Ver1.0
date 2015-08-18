﻿using Mandrill;
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
            IRepository<CampaignProductRecord> campaignProductRepository)
        {
            _mailChimpSettingsRepository = mailChimpSettingsRepository;
            _contentManager = contentManager;
            _messageService = messageService;
            _settingsService = settingsService;
            _notifier = notifier;
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _campaignRepository = campaignRepository;
            _userRolesPartRepository = userRolesPartRepository;
            _payoutsRepository = payoutsRepository;
            _payoutInformRepository = payoutInformRepository;
            _wca = wca;
            _campaignProductRepository = campaignProductRepository;
        }

        public void SendCheckoutRequestEmails(IEnumerable<CheckoutCampaignRequest> checkoutCampaignRequests)
        {
            var pathToTemplates = HttpContext.Current.Server.MapPath(MessageTemplatesPath);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage
            {
                MergeLanguage = MandrillMessageMergeLanguage.Handlebars,
                FromEmail = "noreply@teeyoot.com",
                Subject = "Notification"
            };

            var emails = checkoutCampaignRequests.Select(r => new MandrillMailAddress {Email = r.Email}).ToList();
            mandrillMessage.To = emails;
            var text = File.ReadAllText(pathToTemplates + "make_the_order_buyer.html");
            mandrillMessage.Html = text;
            var res = SendTmplMessage(api, mandrillMessage);
        }

        public void SendExpiredCampaignMessageToSeller(int campaignId, bool isSuccesfull)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            if (isSuccesfull)
            {
                mandrillMessage.Subject = "Campaign reach goal!";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "campaign-is-printing-seller-template.html");
            }
            else
            {
                mandrillMessage.Subject = "Campaign didn't reach goal!";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "not-reach-goal-seller-template.html");
            }

            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            SendTmplMessage(api, mandrillMessage);
        }

        public void SendExpiredCampaignMessageToBuyers(int campaignId, bool isSuccesfull)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            if (isSuccesfull)
            {
                mandrillMessage.Subject = "Your order is printing!";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "order-is-printing-buyer-template.html");
            }
            else
            {
                mandrillMessage.Subject = "Your order was cancelled!";
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "not-reach-goal-buyer-template.html");
            }

            List<LinkOrderCampaignProductRecord> ordersList = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive).ToList();
            var campaign = _campaignRepository.Get(campaignId);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var item in ordersList)
            {
                emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "Buyer"));
                FillUserMergeVars(mandrillMessage, item.OrderRecord);
                FillProductsMergeVars(mandrillMessage, item.OrderRecord.Products, pathToMedia, item.OrderRecord.Email, item.OrderRecord.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, campaignId, item.OrderRecord.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            SendTmplMessage(api, mandrillMessage);
        }


        public void SendLaunchCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.Subject = "New Campaign on Teeyoot";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "launch-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendTermsAndConditionsMessageToSeller()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var campaigns = _campaignRepository.Table.Where(camp => camp.WhenApproved < DateTime.UtcNow.AddDays(-1) && camp.WhenApproved > DateTime.UtcNow.AddDays(-3));
            foreach (var campaign in campaigns)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.Subject = "Teeyoot Terms and Conditions";
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
                mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")};
                FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "terms-conditions-template.html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendCampaignFinished1DayMessageToSeller()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var campaigns = _campaignRepository.Table.Where(camp => camp.EndDate < DateTime.UtcNow.AddDays(-1) && camp.EndDate > DateTime.UtcNow.AddDays(-3));
            foreach (var campaign in campaigns)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.Subject = "Campaing is finished!";
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
                mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")};
                FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "campaign-is-finished-template.html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendOrderShipped3DaysToBuyer()
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
           
            var orders = _orderRepository.Table.Where(order => order.WhenSentOut < DateTime.UtcNow.AddDays(-1) && order.WhenSentOut > DateTime.UtcNow.AddDays(-3));
            foreach (var order in orders)
            {
                var mandrillMessage = new MandrillMessage() { };
                mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                mandrillMessage.FromEmail = ADMIN_EMAIL;
                mandrillMessage.Subject = "Order has been shipped!";
                List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                emails.Add(new MandrillMailAddress(order.Email, "Buyer"));
                mandrillMessage.To = emails;
                FillUserMergeVars(mandrillMessage, order, order.Email);
                FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
                mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "shipped-order-3day-after-template.html");
                SendTmplMessage(api, mandrillMessage);
            }

        }

        public void SendRejectedCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.Subject = "Campaign reject";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "reject-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendNewCampaignAdminMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.Subject = "New Campaign on Teeyoot";
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillCampaignMergeVars(mandrillMessage, campaignId, user.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-campaign-admin-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendCompletedPayoutMessage(string pathToTemplates, string pathToMedia, PayoutRecord payout)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "Payout completed";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == payout.UserId);
            var payoutInf = _payoutInformRepository.Table.Where(inf => inf.TranzactionId == payout.Id).FirstOrDefault();
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(seller.Email, "Seller"));
            FillPayoutRequestMergeVars(mandrillMessage, seller.Email, seller.Id, payoutInf.AccountNumber.ToString(), payoutInf.BankName.ToString(), payoutInf.AccountHolderName.ToString(), payoutInf.ContactNumber.ToString(), "");
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "withdraw-completed-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendChangedCampaignStatusMessage(int campaignId, string campaignStatus)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(seller.Email, "Seller")
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            switch (campaignStatus)
            {
                case "Unpaid":
                    {
                        mandrillMessage.Subject = "Your campaign has been unpaid!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "unpaid-campaign-template.html");
                        break;
                    };
                case "Paid":
                    {
                        mandrillMessage.Subject = "Your campaign has been paid!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "paid-campaign-template.html");
                        break;
                    };
                //case "PartiallyPaid":
                //    {
                //        mandrillMessage.Subject = "Your campaign has been partially paid!";
                //        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "partially-paid-campaign-template.html");
                //        break;
                //    };
            }
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendSellerMessage(int messageId, string pathToMedia, string pathToTemplates)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            var message = _messageService.GetMessage(messageId);
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = message.Sender;
            mandrillMessage.Subject = message.Subject;
            List<LinkOrderCampaignProductRecord> ordersList = _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == message.CampaignId && p.OrderRecord.IsActive).ToList();
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var item in ordersList)
            {
                emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "Seller"));
                FillUserMergeVars(mandrillMessage, item.OrderRecord);
                FillSellerToBuyersProductsMergeVars(mandrillMessage, item.OrderRecord.Products, pathToMedia, item.OrderRecord.Email, item.OrderRecord.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, message.CampaignId, item.OrderRecord.Email, pathToMedia, pathToTemplates);
            }
            mandrillMessage.To = emails;
            string text = System.IO.File.ReadAllText(pathToTemplates + "seller-template.html").Replace("{{Text}}", message.Text);
            mandrillMessage.Html = text;
            message.IsApprowed = true;
            var res = SendTmplMessage(api, mandrillMessage);
        }

        public void SendNewOrderMessageToAdmin(int orderId, string pathToMedia, string pathToTemplates)
        {
            var order = _orderRepository.Get(orderId);
            //string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            //string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "New order";
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
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-order-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendNewOrderMessageToBuyer(int orderId, string pathToMedia, string pathToTemplates)
        {
            var order = _orderRepository.Get(orderId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "New order";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                emails.Add(new MandrillMailAddress(order.Email, "Buyer"));
                FillUserMergeVars(mandrillMessage, order, order.Email);
                FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
                FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "new-order-buyer-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendEditedCampaignMessageToSeller(int campaignId, string pathToMedia, string pathToTemplates)
        {
            var campaign = _campaignRepository.Get(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "Your campaign has been changed";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            emails.Add(new MandrillMailAddress(seller.Email, "Seller"));
            List<CampaignProductRecord> orderedProducts = _campaignProductRepository.Table.Where(prod => prod.CampaignRecord_Id == campaign.Id).ToList();
            FillCampaignProductsMergeVars(mandrillMessage, orderedProducts, pathToMedia, seller.Email);
            FillCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
            FillAdditionalCampaignMergeVars(mandrillMessage, campaign.Id, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "edited-campaign-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendPayoutRequestMessageToAdmin(int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin)
        {
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "noreply@teeyoot.com";
            mandrillMessage.Subject = "Payout Request";
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId);
            var users = _contentManager.GetMany<IUser>(userIds, VersionOptions.Published, QueryHints.Empty);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var user in users)
            {
                emails.Add(new MandrillMailAddress(user.Email, "Admin"));
                FillPayoutRequestMergeVars(mandrillMessage, user.Email, userId, accountNumber, bankName, accHoldName, contNum, messAdmin);
            }
            mandrillMessage.To = emails;
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "withdraw-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendOrderStatusMessage(string pathToTemplates, string pathToMedia, int orderId, string orderStatus)
        {
            var order = _orderRepository.Get(orderId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            switch (orderStatus)
            {
                case "Approved":
                    {
                        mandrillMessage.Subject = "Your order has been approved!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "place-order-template.html");
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
                        mandrillMessage.Subject = "The items you orders are now printing!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "order-is-printing-buyer-template.html");
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
                        mandrillMessage.Subject = "Your order has been shipped!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "shipped-order-template.html");
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
                        mandrillMessage.Subject = "Your order has been delivered!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "delivered-order-template.html");
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
                        mandrillMessage.Subject = "Your order has been cancelled!";
                        mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "cancelled-order-template.html");
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
           
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = ADMIN_EMAIL;
            mandrillMessage.Subject = "Recover Order on Teeyoot";           
            mandrillMessage.To = new List<MandrillMailAddress>(){
                new MandrillMailAddress(email, "Buyer")
            };
            FillOrdersMergeVars(mandrillMessage, orders, email, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "recover_orders_for_buyer.html");
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

            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;
                baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
            }
            else
            {
                baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
            }

            var campaign = _campaignRepository.Get(campaignId);
            message.AddRcptMergeVars(email, "CampaignTitle", campaign.Title);
            message.AddRcptMergeVars(email, "Url", baseUrl);
            message.AddRcptMergeVars(email, "CampaignAlias", campaign.Alias);
            message.AddRcptMergeVars(email, "ReservedCount", campaign.ProductCountSold.ToString());
            message.AddRcptMergeVars(email, "Goal", campaign.ProductCountGoal.ToString());
            message.AddRcptMergeVars(email, "SellerEmail", _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId).Email);
            message.AddRcptMergeVars(email, "CampaignPreviewUrl", pathToMedia + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products[0].Id + "/normal/front.png");

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
            if (record.TotalPriceWithPromo > 0.0)
            {
                message.AddRcptMergeVars(record.Email, "TOTALPRICE", record.TotalPriceWithPromo.ToString());
            }
            else
            {
                message.AddRcptMergeVars(record.Email, "TOTALPRICE", record.TotalPrice.ToString());
            }

        }

        private void FillPayoutRequestMergeVars(MandrillMessage message, string adminEmail, int userId, string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin)
        {

            var requester = _contentManager.Get<TeeyootUserPart>(userId, VersionOptions.Latest);;

            message.AddRcptMergeVars(adminEmail, "Requester_Name", requester.PublicName);
            message.AddRcptMergeVars(adminEmail, "AccountNumber", accountNumber);
            message.AddRcptMergeVars(adminEmail, "BankName", bankName);
            message.AddRcptMergeVars(adminEmail, "AccHolderName", accHoldName);
            message.AddRcptMergeVars(adminEmail, "ContactNumber", contNum);
            message.AddRcptMergeVars(adminEmail, "Text", messAdmin);
         

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
            if (record.TotalPriceWithPromo > 0.0)
            {
                message.AddRcptMergeVars(adminEmail, "TOTALPRICE", record.TotalPriceWithPromo.ToString());
            }
            else
            {
                message.AddRcptMergeVars(adminEmail, "TOTALPRICE", record.TotalPrice.ToString());
            }

        }

        private void FillProductsMergeVars(MandrillMessage message, IList<LinkOrderCampaignProductRecord> orderedProducts, string pathToMedia, string email, string orderPublicId)
        {
            List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();
            foreach (var item in orderedProducts)
            {

                int index = orderedProducts.IndexOf(item);
                int idSize = item.ProductSizeRecord.Id;
                float costSize = item.CampaignProductRecord.ProductRecord.SizesAvailable.Where(c => c.ProductSizeRecord.Id == idSize).First().SizeCost;
                float price = (float)item.CampaignProductRecord.Price + costSize;
                products.Add(new Dictionary<string, object>{                 
                        {"quantity", item.Count},
                        {"name",  item.CampaignProductRecord.ProductRecord.Name},
                        {"description",  item.CampaignProductRecord.ProductRecord.Details},
                        {"price", price},
                        {"size", item.ProductSizeRecord.SizeCodeRecord.Name},
                        {"currency", item.OrderRecord.CurrencyRecord.Code},
                        {"total_price", price* item.Count},
                        {"preview_url", pathToMedia + "/Media/campaigns/" + item.CampaignProductRecord.CampaignRecord_Id + "/" + item.CampaignProductRecord.Id + "/normal/front.png"}
                     });

            }
            var arr = products.ToArray();
            message.AddRcptMergeVars(email, "PRODUCTS", products.ToArray());
            message.AddRcptMergeVars(email, "orderPublicId", orderPublicId);
        }

        private void FillCampaignProductsMergeVars(MandrillMessage message, IList<CampaignProductRecord> campaignProducts, string pathToMedia, string email)
        {
            List<Dictionary<string, object>> products = new List<Dictionary<string, object>>();
            foreach (var item in campaignProducts)
            {
           
                products.Add(new Dictionary<string, object>{                 
                        {"name",  item.ProductRecord.Name},
                        {"price", item.Price},
                        {"currency", item.CurrencyRecord.Code},
                        {"preview_url", pathToMedia + "/Media/campaigns/" + item.CampaignRecord_Id + "/" + item.Id + "/normal/front.png"}
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
