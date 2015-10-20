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
using Orchard.Localization;
using Orchard;
using Teeyoot.Module.Services.Interfaces;

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
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        private dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public AdminMessageContentController(IMessageService messageService, ISiteService siteService, IShapeFactory shapeFactory, INotifier notifier,
            ICampaignService campaignService,
            IMailChimpSettingsService settingsService,
            IOrderService orderService,
            IWorkContextAccessor wca,
            ITeeyootMessagingService teeyootMessagingService
            )
        {
            _messageService = messageService;
            _siteService = siteService;
            _notifier = notifier;
            _campaignService = campaignService;
            _settingsService = settingsService;
            _orderService = orderService;
            _wca = wca;
            _teeyootMessagingService = teeyootMessagingService;

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
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            _teeyootMessagingService.SendSellerMessage(messageId, pathToMedia, pathToTemplates);
            _notifier.Information(T("Message has been sent!"));
            return RedirectToAction("Index", "AdminMessageContent");

        }


    }
}