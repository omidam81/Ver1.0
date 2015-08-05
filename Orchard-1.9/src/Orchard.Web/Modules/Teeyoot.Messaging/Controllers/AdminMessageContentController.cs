using Mandrill;
using Mandrill.Model;
using Orchard.DisplayManagement;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Messaging.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Orchard.UI.Notify;
using System.IO;
using System.Threading;
using Teeyoot.Messaging.Models;
using Orchard.Localization;
using Teeyoot.Messaging.Services;
using Orchard;

namespace Teeyoot.Messaging.Controllers
{
    [Admin]
    public class AdminMessageContentController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ISiteService _siteService;
        private readonly INotifier _notifier;
        private readonly ICampaignService _campaignService;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IOrderService _orderService;
        private readonly IWorkContextAccessor _wca;
        private readonly IMandrillService _mandrillService;
        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminMessageContentController(IMessageService messageService, ISiteService siteService, IShapeFactory shapeFactory, INotifier notifier,
            ICampaignService campaignService,
            IMailChimpSettingsService settingsService,
            IOrderService orderService,
            IWorkContextAccessor wca,
            IMandrillService mandrillService
            )
        {
            _messageService = messageService;
            _siteService = siteService;
            _notifier = notifier;
            _campaignService = campaignService;
            _settingsService = settingsService;
            _orderService = orderService;
            _wca = wca;
            _mandrillService = mandrillService;
            Shape = shapeFactory;
        }

        // GET: AdminMessageContent
        public ActionResult Index(PagerParameters pagerParameters, AdminMessagesViewModel adminViewModel)
        {
            var messages = _messageService.GetAllMessages().Where(s => s.IsApprowed == false).ToList();
            var entriesProjection = messages.Select(e =>
            {
                return Shape.MessageEntry(
                    SendDate: e.SendDate,
                    Id: e.Id,
                    Subject: e.Subject,
                    Sender: e.Sender,
                    Text: e.Text,
                    UserId: e.UserId
                    );
            });

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            dynamic pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());
            return View("Index", new AdminMessagesViewModel { Messages = entries.ToArray(), Pager = pagerShape });
        }

        public ActionResult SendSellerMessage(int messageId)
        {
            //var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            //var api = new MandrillApi(record.ApiKey);
            //var mandrillMessage = new MandrillMessage() { };
            //var message = _messageService.GetMessage(messageId);
            //mandrillMessage.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
            //mandrillMessage.FromEmail = message.Sender;
            //mandrillMessage.Subject = message.Subject;
            //List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(message.CampaignId).ToList();
            //var campaign = _campaignService.GetCampaignById(message.CampaignId);
            //List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
            //foreach (var item in ordersList)
            //{
            //    emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
            //    FillMessageMergeVars(mandrillMessage, item);
            //}
            //mandrillMessage.To = emails;
            //string text = System.IO.File.ReadAllText(Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/seller-template.html"));
            //string messageText = text.Replace("---MessageContent---", message.Text);
            //messageText = messageText.Replace("---SellerEmail---", message.Sender);
            //messageText = messageText.Replace("---CampaignTitle---", campaign.Title);
            //string previewUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Media/campaigns/" + message.CampaignId + "/" + campaign.Products[0].Id + "/normal/front.png";
            //messageText = messageText.Replace("---CampaignPreviewUrl---", previewUrl);
            //mandrillMessage.Html = messageText;
            //var res = SendTmplMessage(api, mandrillMessage);
            //_notifier.Information(T("Message has been sent!"));
            //message.IsApprowed = true;
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _mandrillService.SendSellerMessage(messageId, pathToTemplates, pathToMedia);
            return RedirectToAction("Index", "AdminMessageContent");

        }

        static void FillMessageMergeVars(MandrillMessage message, LinkOrderCampaignProductRecord record)
        {


            //var products = new Dictionary<string, object>
            //        {
            //            {"quantity", record.Count},
            //            {"name",  record.CampaignProductRecord.ProductRecord.Name},
            //            {"description",  record.CampaignProductRecord.ProductRecord.Details},
            //            {"price",  record.CampaignProductRecord.Price}
            //        };

            //message.AddRcptMergeVars(record.OrderRecord.Email, "FNAME", record.OrderRecord.FirstName);
            //message.AddRcptMergeVars(record.OrderRecord.Email, "LNAME", record.OrderRecord.LastName);
            //message.AddRcptMergeVars(record.OrderRecord.Email, "CITY", record.OrderRecord.City);
            //message.AddRcptMergeVars(record.OrderRecord.Email, "STATE", record.OrderRecord.State);
            //message.AddRcptMergeVars(record.OrderRecord.Email, "COUNTRY", record.OrderRecord.Country);
            //if (record.OrderRecord.TotalPriceWithPromo > 0.0)
            //{
            //    message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPriceWithPromo.ToString());
            //}
            //else
            //{
            //    message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPrice.ToString());
            //}
            //message.AddRcptMergeVars(record.OrderRecord.Email, "PRODUCTS", products);
        }

        static string SendTmplMessage(MandrillApi mAPI, MandrillMessage message)
        {
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }

        public void SendOrderMessage()
        {
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _mandrillService.SendOrderMessage(49, pathToTemplates, pathToMedia);
        }

    }
}