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
            //Вытаскивать валюту по культуре 
            model.Currency = _currencyRepository.Table.ToList().ElementAt(0).Code;
            model.CurrencyId = _currencyRepository.Table.ToList().ElementAt(0).Id;
            model.Transactions = list;
            foreach(var item in model.Transactions){
                if (item.IsPlus && item.Status != "pending")
                    model.Balance = model.Balance + item.Amount;
                else if (!item.IsPlus && item.Status != "pending")
                    model.Balance = model.Balance - item.Amount;
            }

            if (model.Balance < 0)
                model.Balance = 0;

            return View(model);
        }

        public ActionResult StartPayout() {
            return View();
        }

       [HttpPost]
        public ActionResult SendMail(string accountNumber, string bankName, string accHoldName, string contNum, string messAdmin, int currId)
        {
            int currentUserId = Services.WorkContext.CurrentUser.Id;

            var payouts = _payoutService.GetAllPayouts().ToList();
            double balance = 0;
            foreach (var item in payouts)
            {
                if (item.IsPlus && item.UserId == currentUserId)
                    balance = balance + item.Amount;
                else if (item.UserId == currentUserId)
                    balance = balance - item.Amount;
            }
            if (balance > 0)
            {
                var payout = new PayoutRecord() { Date = DateTime.Now, Amount = balance, Event = T("You requested a payout").ToString(),Currency_Id = currId, IsPlus = false, UserId = currentUserId, Status = "pending" };
                _payoutService.AddPayout(payout);
                _paymentInfService.AddPayment(new PaymentInformationRecord { AccountNumber = Convert.ToInt32(accountNumber),
                    AccountHolderName = accHoldName,
                    BankName = bankName,
                    ContactNumber = Convert.ToInt32(contNum),
                    MessAdmin = messAdmin,
                    TranzactionId = payout.Id});
            
            }
            _teeyootMessagingService.SendPayoutRequestMessageToAdmin(currentUserId, accountNumber, bankName, accHoldName, contNum, messAdmin);
            
            return RedirectToAction("Payouts");
        }
    
    
    }
}