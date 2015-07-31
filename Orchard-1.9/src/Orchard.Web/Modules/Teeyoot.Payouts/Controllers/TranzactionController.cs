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

namespace Teeyoot.Payouts.Controllers
{
    [Admin]
    public class TranzactionController : Controller
    {
        private readonly IPayoutService _payoutService;
        // GET: Tranzaction

        public TranzactionController(IPayoutService payoutService)                            
        {
            _payoutService = payoutService;
        }




        public ActionResult Index()
        {
            var payouts = _payoutService.GetAllPayouts();
            var list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).ToList();
            var model = new PayoutsViewModel();
            model.Transactions = list;
            return View(model);
        }


        public ActionResult EditStatus(int id) 
        {
            var payouts = _payoutService.GetAllPayouts().ToList();
            foreach (var item in payouts) {
                if (item.Id == id)
                    item.Status = "completed";
                _payoutService.UpdatePayout(item);
            }

            return RedirectToAction("Index");
        }
    }
}