using Mandrill;
using Mandrill.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Orchard.UI.Notify;
using Orchard.Localization;
using Orchard.ContentManagement;
using Orchard.Data;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Messaging.Models;


namespace Teeyoot.Messaging.Services
{
    public class MandrillService : Controller, IMandrillService
    {
        private readonly IRepository<MailChimpSettingsPartRecord> _mailChimpSettingsRepository;
        private readonly IContentManager _contentManager;
        private readonly IMessageService _messageService;
        private readonly ICampaignService _campaignService;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IOrderService _orderService;
        private readonly INotifier _notifier;
        public Localizer T { get; set; }

        public MandrillService(IRepository<MailChimpSettingsPartRecord> mailChimpSettingsRepository, IContentManager contentManager,ICampaignService campaignService,
            IMailChimpSettingsService settingsService,
            IMessageService messageService,
             INotifier notifier,
            IOrderService orderService)
        {
            _mailChimpSettingsRepository = mailChimpSettingsRepository;
            _contentManager = contentManager;
            _messageService = messageService;
            _campaignService = campaignService;
            _settingsService = settingsService;
            _notifier = notifier;
            _orderService = orderService;
        }


        public void SendSellerMessage(int messageId, string pathToTemplates, string pathToMedia)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            var message = _messageService.GetMessage(messageId);
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = message.Sender;
            mandrillMessage.Subject = message.Subject;
            List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(message.CampaignId).ToList();
            var campaign = _campaignService.GetCampaignById(message.CampaignId);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var item in ordersList)
            {
                emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
                FillMessageMergeVars(mandrillMessage, item);
            }
            mandrillMessage.To = emails;
            string text = System.IO.File.ReadAllText(pathToTemplates+"seller-template.html");
            string messageText = text.Replace("---MessageContent---", message.Text);
            messageText = messageText.Replace("---SellerEmail---", message.Sender);
            messageText = messageText.Replace("---CampaignTitle---", campaign.Title);
            string previewUrl = pathToMedia + "/Media/campaigns/" + message.CampaignId + "/" + campaign.Products[0].Id + "/normal/front.png";
            messageText = messageText.Replace("---CampaignPreviewUrl---", previewUrl);
            mandrillMessage.Html = messageText;
            var res = SendTmplMessage(api, mandrillMessage);
            _notifier.Information(T("Message has been sent!"));
            message.IsApprowed = true;
        }

        public void FillMessageMergeVars(MandrillMessage message, Module.Models.LinkOrderCampaignProductRecord record)
        {
            var products = new Dictionary<string, object>
                    {
                        {"quantity", record.Count},
                        {"name",  record.CampaignProductRecord.ProductRecord.Name},
                        {"description",  record.CampaignProductRecord.ProductRecord.Details},
                        {"price",  record.CampaignProductRecord.Price},
                        {"total_price", record.OrderRecord.TotalPrice}
                    };

            message.AddRcptMergeVars(record.OrderRecord.Email, "FNAME", record.OrderRecord.FirstName);
            message.AddRcptMergeVars(record.OrderRecord.Email, "LNAME", record.OrderRecord.LastName);
            message.AddRcptMergeVars(record.OrderRecord.Email, "CITY", record.OrderRecord.City);
            message.AddRcptMergeVars(record.OrderRecord.Email, "STATE", record.OrderRecord.State);
            message.AddRcptMergeVars(record.OrderRecord.Email, "COUNTRY", record.OrderRecord.Country);
            if (record.OrderRecord.TotalPriceWithPromo > 0.0)
            {
                message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPriceWithPromo.ToString());
            }
            else
            {
                message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPrice.ToString());
            }
            message.AddRcptMergeVars(record.OrderRecord.Email, "PRODUCTS", products);
        }

        public string SendTmplMessage(MandrillApi mAPI, Mandrill.Model.MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }


        public void SendWelcomeMessage(string userEmail, string pathToTemplates)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "admin@teeyoot.com";
            mandrillMessage.Subject = "Welcome to teeyoot!";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(userEmail, "user"));
            mandrillMessage.To = emails;
            string text = System.IO.File.ReadAllText(pathToTemplates + "welcome-template.html");
            mandrillMessage.Html = text;
            var res = SendTmplMessage(api, mandrillMessage);
            var result = res;
        }


        public void SendOrderMessage(int campaignId, string pathToTemplates, string pathToMedia)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "admin@teeyoot.com";
            mandrillMessage.Subject = "Teeyoot order";
            List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(campaignId).ToList();
            var campaign = _campaignService.GetCampaignById(campaignId);
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            foreach (var item in ordersList)
            {
                emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
                FillMessageMergeVars(mandrillMessage, item);
            }
            mandrillMessage.To = emails;
            string text = System.IO.File.ReadAllText(pathToTemplates + "confirm-order-template.html");
            //string messageText = text.Replace("---MessageContent---", message.Text);
            //messageText = messageText.Replace("---SellerEmail---", message.Sender);
            //messageText = messageText.Replace("---CampaignTitle---", campaign.Title);
            string previewUrl = pathToMedia + "/Media/campaigns/" + campaignId + "/" + campaign.Products[0].Id + "/normal/front.png";
            //messageText = messageText.Replace("---CampaignPreviewUrl---", previewUrl);
            mandrillMessage.Html = text;
            var res = SendTmplMessage(api, mandrillMessage);
        }
    }
}