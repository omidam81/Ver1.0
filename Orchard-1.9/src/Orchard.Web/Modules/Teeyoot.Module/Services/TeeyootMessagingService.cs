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

        private void FillCampaignMergeVars(MandrillMessage message, int campaignId, string email, string pathToMedia, string pathToTemplates)
        {

            var campaign = _campaignService.GetCampaignById(campaignId);
            message.AddRcptMergeVars(email, "CampaignTitle", campaign.Title);
            message.AddRcptMergeVars(email, "CampaignAlias", campaign.Alias);
            message.AddRcptMergeVars(email, "preview_url", pathToMedia + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products[0].Id + "/normal/front.png");

        }

        private string SendTmplMessage(MandrillApi mAPI, Mandrill.Model.MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }

    }
}
