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

namespace Teeyoot.Payouts.Controllers
{
    [Admin]
    public class TranzactionController : Controller
    {
        private readonly IPayoutService _payoutService;
        private readonly ISiteService _siteService;
        private readonly IPaymentInformationService _paymentInformationService;
        private readonly IContentManager _contentManager;

        private dynamic Shape { get; set; }
        // GET: Tranzaction

        public TranzactionController(IPayoutService payoutService,
                                     ISiteService siteService,
                                     IShapeFactory shapeFactory,
                                     IMembershipService membershipService,
                                     IContentManager contentManager,
                                     IPaymentInformationService paymentInformationService)
        {
            _payoutService = payoutService;
            _siteService = siteService;
            _paymentInformationService = paymentInformationService;
            _contentManager = contentManager;
            Shape = shapeFactory;
        }




        public ActionResult Index(PagerParameters pagerParameters, PayoutsViewModel adminViewModel, string filter = "")
        {
            var payouts = _payoutService.GetAllPayouts();
            List<History> list;
            if (filter == "1")
                list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "pending").ToList();
            else if (filter == "2")
                list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(s => s.Status == "completed").ToList();
            else
                list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).ToList();

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


        public ActionResult EditStatus(int id)
        {
            var payouts = _payoutService.GetAllPayouts().ToList();
            foreach (var item in payouts)
            {
                if (item.Id == id)
                    item.Status = "completed";
                _payoutService.UpdatePayout(item);
            }

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