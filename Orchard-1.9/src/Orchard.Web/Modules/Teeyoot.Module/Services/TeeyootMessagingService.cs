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
using Orchard.Users.Models;
using System.Web.Mvc;


namespace Teeyoot.Messaging.Services
{
    public class TeeyootMessagingService : ITeeyootMessagingService
    {
        private readonly IRepository<MailChimpSettingsPartRecord> _mailChimpSettingsRepository;
        private readonly IContentManager _contentManager;
        private readonly IMessageService _messageService;
        private readonly ICampaignService _campaignService;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IOrderService _orderService;
        private readonly INotifier _notifier;
        public Localizer T { get; set; }

        public TeeyootMessagingService(IRepository<MailChimpSettingsPartRecord> mailChimpSettingsRepository, IContentManager contentManager, ICampaignService campaignService,
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


        public void SendLaunchCampaignMessage(string pathToTemplates, string pathToMedia, int campaignId)
        {
            var campaign = _campaignService.GetCampaignById(campaignId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "admin@teeyoot.com";
            mandrillMessage.Subject = "New Campaign on Teeyoot";
            var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
            mandrillMessage.To = new List<MandrillMailAddress>()
            {
                new MandrillMailAddress(seller.Email)
            };
            FillCampaignMergeVars(mandrillMessage, campaignId, seller.Email, pathToMedia, pathToTemplates);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "launch-template.html");
            SendTmplMessage(api, mandrillMessage);

        }

        public void SendApproveOrderMessage(string pathToTemplates, string pathToMedia, int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            var api = new MandrillApi(record.ApiKey);
            MandrillMessage mandrillMessage = InitMandrillMessage(order);            
            FillUserMergeVars(mandrillMessage, order);
            FillCampaignMergeVars(mandrillMessage, order.Products[0].CampaignProductRecord.CampaignRecord_Id, order.Email, pathToMedia, pathToTemplates);
            FillProductsMergeVars(mandrillMessage, order.Products, pathToMedia, order.Email, order.OrderPublicId);
            mandrillMessage.Html = System.IO.File.ReadAllText(pathToTemplates + "place-order-template.html");
            SendTmplMessage(api, mandrillMessage);
        }

        private void FillCampaignMergeVars(MandrillMessage message, int campaignId, string email, string pathToMedia, string pathToTemplates)
        {

            var campaign = _campaignService.GetCampaignById(campaignId);
            message.AddRcptMergeVars(email, "CampaignTitle", campaign.Title);
            message.AddRcptMergeVars(email, "CampaignAlias", campaign.Alias);
            message.AddRcptMergeVars(email, "CampaignPreviewUrl", pathToMedia + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products[0].Id + "/normal/front.png");

        }

        private string SendTmplMessage(MandrillApi mAPI, Mandrill.Model.MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }

        private void FillUserMergeVars(MandrillMessage message, OrderRecord record)
        {

            message.AddRcptMergeVars(record.Email, "FNAME", record.FirstName);
            message.AddRcptMergeVars(record.Email, "LNAME", record.LastName);
            message.AddRcptMergeVars(record.Email, "CITY", record.City);
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
                        {"preview_url", pathToMedia + "/Media/campaigns/" + item.CampaignProductRecord.CampaignRecord_Id + "/" + item.CampaignProductRecord.Id + "/normal/front.png"}
                     });

            }
            var arr = products.ToArray();
            message.AddRcptMergeVars(email, "PRODUCTS", products.ToArray());
            message.AddRcptMergeVars(email, "orderPublicId", orderPublicId);
        }


        private MandrillMessage InitMandrillMessage(OrderRecord order)
        {
            var mandrillMessage = new MandrillMessage() { };
            mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            mandrillMessage.FromEmail = "admin@teeyoot.com";
            mandrillMessage.Subject = "Your order";
            List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            emails.Add(new MandrillMailAddress(order.Email));
            mandrillMessage.To = emails;
            return mandrillMessage;
        }

    }
}
