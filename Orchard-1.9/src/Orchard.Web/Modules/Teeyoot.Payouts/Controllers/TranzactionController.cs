using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Admin;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using System.Collections.Generic;
using Orchard.UI.Navigation;
using Orchard.Settings;
using Orchard.DisplayManagement.Shapes;
using Orchard.DisplayManagement;
using Orchard.Users.Models;
using System.IO;
using System;
using System.Net;

namespace Teeyoot.Payouts.Controllers
{
    [Admin]
    public class TranzactionController : Controller
    {
        private readonly IPayoutService _payoutService;
        private readonly ISiteService _siteService;
        private readonly IPaymentInformationService _paymentInformationService;
        private readonly IContentManager _contentManager;
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        private dynamic Shape { get; set; }
        // GET: Tranzaction

        public TranzactionController(IPayoutService payoutService,
                                     ISiteService siteService,
                                     IShapeFactory shapeFactory,
                                     IContentManager contentManager,
                                     IPaymentInformationService paymentInformationService,
                                     ITeeyootMessagingService teeyootMessagingService)
        {
            _payoutService = payoutService;
            _siteService = siteService;
            _paymentInformationService = paymentInformationService;
            _contentManager = contentManager;
            _teeyootMessagingService = teeyootMessagingService;
            Shape = shapeFactory;
        }




        public ActionResult Index(PagerParameters pagerParameters, PayoutsViewModel adminViewModel, string filter = "")
        {
            var payouts = _payoutService.GetAllPayouts();
            List<History> list;
            if (payouts != null)
            {
                if (filter == "1")
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "pending" && s.IsPlus == true).ToList();
                else if (filter == "2")
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "Completed" && s.IsPlus == true).ToList();
                else
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.IsPlus == true).ToList();

                var entriesProjection = list.Select(e =>
                {
                    return Shape.FaqEntry(
                        Date: e.Date,
                        Id: e.Id,
                        Event: e.Event,
                        Amount: e.Amount,
                        Status: e.Status,
                        UserId: e.UserId,
                        IsPlus: e.IsPlus
                        );
                });
                var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
                var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
                var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

                return View("Index", new PayoutsViewModel { Transacts = entries.ToArray(), Pager = pagerShape });
            }

            return View("Index", new PayoutsViewModel { Transacts = null, Pager = null });
        }

        public ActionResult Payouts(PagerParameters pagerParameters, PayoutsViewModel adminViewModel, string filter = "")
        {
            var payouts = _payoutService.GetAllPayouts();
            List<History> list;
            if (payouts != null)
            {
                if (filter == "1")
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "pending" && s.IsPlus == false).ToList();
                else if (filter == "2")
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "Completed" && s.IsPlus == false).ToList();
                else
                    list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.IsPlus == false).ToList();

                var entriesProjection = list.Select(e =>
                {
                    return Shape.FaqEntry(
                        Date: e.Date,
                        Id: e.Id,
                        Event: e.Event,
                        Amount: e.Amount,
                        Status: e.Status,
                        UserId: e.UserId,
                        IsPlus: e.IsPlus
                        );
                });
                var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
                var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
                var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

                return View("Payouts", new PayoutsViewModel { Transacts = entries.ToArray(), Pager = pagerShape });
            }

            return View("Payouts", new PayoutsViewModel { Transacts = null, Pager = null });
        }


        [HttpPost]
        public HttpStatusCodeResult EditPayout(int Id, double Cost) 
        {
            var payout = _payoutService.GetAllPayouts().FirstOrDefault(p => p.Id == Id);
            payout.Amount = Cost;
            _payoutService.UpdatePayout(payout);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult EditStatus(int id)
        {
            var item = _payoutService.GetAllPayouts().Where(payout => payout.Id == id).First();
            item.Status = "completed";
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            _payoutService.UpdatePayout(item);
            _teeyootMessagingService.SendCompletedPayoutMessage(pathToTemplates, pathToMedia, item);
            return RedirectToAction("Index");
        }

        public JsonResult GetPayoutInfirmation(int userId, int tranzId)
        {
            var usr = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == userId);
            var tranz = _paymentInformationService.GetAllPayments().FirstOrDefault(t => t.TranzactionId == tranzId);
            if(tranz == null)
                return Json(new {userName = usr.UserName,email = usr.Email, accountNumber = "", accountHolderName = "",bankName = "", contactNumber = "", mesAdmin = "" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new {userName = usr.UserName,email = usr.Email,accountNumber = tranz.AccountNumber, accountHolderName = tranz.AccountHolderName, bankName = tranz.BankName,contactNumber = tranz.ContactNumber,mesAdmin = tranz.MessAdmin},JsonRequestBehavior.AllowGet);
        }

    }
}