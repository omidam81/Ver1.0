using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Payouts()
        {
            var payouts = _payoutService.GetAllPayouts();
            var list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId }).ToList();
            var model = new PayoutsViewModel();
            model.Transactions = list;
            foreach(var item in model.Transactions){
                if (item.IsPlus)
                    model.Balance = model.Balance + item.Amount;
                else
                    model.Balance = model.Balance - item.Amount;
            }
            return View(model);
        }

        public ActionResult StartPayout() {
            return View();
        }
    }
}