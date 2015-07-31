using System;
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
            int currentUserId = Services.WorkContext.CurrentUser.Id;
            var payouts = _payoutService.GetAllPayouts();
            var list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status }).Where(t=>t.UserId == currentUserId).ToList();
            var model = new PayoutsViewModel();
            model.Transactions = list;
            foreach(var item in model.Transactions){
                if (item.IsPlus && item.Status != "pending")
                    model.Balance = model.Balance + item.Amount;
                else if (!item.IsPlus && item.Status != "pending")
                    model.Balance = model.Balance - item.Amount;
            }
            return View(model);
        }

        public ActionResult StartPayout() {
            return View();
        }
    
        public ActionResult sendMail()
        {
            var payouts = _payoutService.GetAllPayouts().ToList();
            double balance = 0;
            foreach (var item in payouts)
            {
                if (item.IsPlus)
                    balance = balance + item.Amount;
                else
                    balance = balance - item.Amount;
            }

            _payoutService.AddPayout(new PayoutRecord() {Date = DateTime.Now, Amount = balance,Event = "You requested a payout", IsPlus = false, UserId = 451, Status = "pending"});

            return RedirectToAction("Payouts");
        }
    
    
    }
}