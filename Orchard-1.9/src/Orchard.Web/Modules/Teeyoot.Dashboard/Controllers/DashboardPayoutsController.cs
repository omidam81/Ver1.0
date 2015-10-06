using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        public ActionResult Accounts()
        {
            int currentUserId = Services.WorkContext.CurrentUser.Id;
            var payouts = _payoutService.GetAllPayouts();
            var list = payouts.Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status, CurrencyId = s.Currency_Id }).Where(t => t.UserId == currentUserId).ToList();
            var model = new PayoutsViewModel();
            //Вытаскивать валюту по культуре 
            //model.Currency = _currencyRepository.Table.ToList().ElementAt(0).Code;
            //model.CurrencyId = _currencyRepository.Table.ToList().ElementAt(0).Id;
            model.Transactions = list;
            model.Balances = _currencyRepository.Table.Select(c => new Balance { CurrencyId = c.Id, Currency = c.Code }).ToList();
            var procProfits = 0.0;
            var unclProfits = 0.0;
            foreach (var itemBal in model.Balances)
            {
                foreach (var item in model.Transactions)
                {
                    if (itemBal.CurrencyId == item.CurrencyId)
                    {
                        item.Currency = itemBal.Currency;

                        if (item.IsPlus && item.Status != "Pending")
                        {
                            itemBal.Bal = itemBal.Bal + item.Amount;
                            unclProfits = unclProfits - item.Amount;
                        }

                        else if (!item.IsPlus && item.Status != "Pending")
                        {
                            itemBal.Bal = itemBal.Bal - item.Amount;
                        }
                        else if (!item.IsPlus && item.Status == "Pending")
                        {
                            itemBal.Bal = itemBal.Bal - item.Amount;
                            procProfits = procProfits + item.Amount;
                        }
                    }
                }
                if (itemBal.Bal < 0)
                    itemBal.Bal = 0;

                itemBal.ProcessedProfits = procProfits;
            }

            var campaigns = _campaignService.GetCampaignsOfUser(currentUserId);

            
            foreach (var item in campaigns)
            {
                if (item.ProductMinimumGoal <= item.ProductCountSold)
                	{
                        unclProfits = unclProfits + _orderService.GetProfitActiveOrdersOfCampaign(item.Id);
	                }
            }
            
            if (unclProfits < 0)
                unclProfits = unclProfits * -1;

            model.Balances.Where(b => b.Currency == "RM").First().UnclProfits = Math.Round(unclProfits, 2);



            return View(model);
        }

        public ActionResult StartPayout()
        {
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
                if (item.Currency_Id == currId)
                {
                    if (item.IsPlus && item.UserId == currentUserId)
                        balance = balance + item.Amount;
                    else if (item.UserId == currentUserId)
                        balance = balance - item.Amount;
                }
            }
            if (balance > 0)
            {
                var payout = new PayoutRecord() { Date = DateTime.Now, Amount = balance, Event = T("Someone requested a payout").ToString(), Currency_Id = currId, IsPlus = false, UserId = currentUserId, Status = "Pending" };
                _payoutService.AddPayout(payout);
                _paymentInfService.AddPayment(new PaymentInformationRecord
                {
                    AccountNumber = accountNumber,
                    AccountHolderName = accHoldName,
                    BankName = bankName,
                    ContactNumber = contNum,
                    MessAdmin = messAdmin,
                    TranzactionId = payout.Id
                });

            }
            _teeyootMessagingService.SendPayoutRequestMessageToAdmin(currentUserId, accountNumber, bankName, accHoldName, contNum, messAdmin);
            _teeyootMessagingService.SendPayoutRequestMessageToSeller(currentUserId, accountNumber, bankName, accHoldName, contNum);
            return RedirectToAction("Accounts");
        }


    }
}