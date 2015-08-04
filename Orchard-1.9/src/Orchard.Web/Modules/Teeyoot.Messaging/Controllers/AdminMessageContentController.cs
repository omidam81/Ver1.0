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

namespace Teeyoot.Messaging.Controllers
{
    [Admin]
    public class AdminMessageContentController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ISiteService _siteService;
        private dynamic Shape { get; set; }

        public AdminMessageContentController(IMessageService messageService, ISiteService siteService, IShapeFactory shapeFactory)
        {
            _messageService = messageService;
            _siteService = siteService;
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
    }
}