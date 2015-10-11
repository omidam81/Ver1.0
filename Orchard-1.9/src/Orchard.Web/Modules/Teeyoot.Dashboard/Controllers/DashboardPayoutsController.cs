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
            var list = payouts.Where(t => t.UserId == currentUserId && t.IsOrder == false).ToList();
            var model = new PayoutsViewModel();
            //Вытаскивать валюту по культуре 
            //model.Currency = _currencyRepository.Table.ToList().ElementAt(0).Code;
            //model.CurrencyId = _currencyRepository.Table.ToList().ElementAt(0).Id;
            //model.Transactions = list;
            model.Balances = _currencyRepository.Table.Select(c => new Balance { CurrencyId = c.Id, Currency = c.Code }).ToList();

            var unclProfits = 0.0;
            var balance = 0.0;
            var procProfits = 0.0;
            //var allPaymentProfit = 0.0;
            foreach (var itemBal in model.Balances)
            {
                foreach (var item in list)
                {
                    if (itemBal.CurrencyId == item.Currency_Id)
                    {

                        if (item.Status == "Pending")
                        {
                            procProfits = procProfits + item.Amount;
                            unclProfits = unclProfits - item.Amount;
                            balance = balance - item.Amount;
                        }
                        if (item.IsProfitPaid != null && item.IsProfitPaid && item.Status != "Pending")
                        {
                            unclProfits = unclProfits - item.Amount;
                            balance = balance - item.Amount;
                        }
                        //if (item.IsPlus && item.Status != "Pending")
                        //{
                        //itemBal.Bal = itemBal.Bal + item.Amount;
                        //unclProfits = unclProfits - item.Amount;
                        //}

                        //else if (!item.IsPlus && item.Status != "Pending")
                        //{
                        //    itemBal.Bal = itemBal.Bal - item.Amount;
                        //}
                        //else if (!item.IsPlus && item.Status == "Pending")
                        //{
                        //itemBal.Bal = itemBal.Bal - item.Amount;
                        //procProfits = procProfits + item.Amount;
                        //}
                    }
                }
                //if (itemBal.Bal < 0)
                //    itemBal.Bal = 0;

                itemBal.ProcessedProfits = procProfits;
            }

            var campaigns = _campaignService.GetCampaignsOfUser(currentUserId);
            var campaignsInProfit = new List<CampaignRecord>();

            foreach (var item in campaigns)
            {
                if (item.ProductMinimumGoal <= item.ProductCountSold)
                {
                    unclProfits = unclProfits + _orderService.GetProfitActiveOrdersOfCampaign(item.Id);
                }

                if (!item.IsActive && _orderService.IsOrdersForCampaignHasStatusDeliveredAndPaid(item.Id))
                {
                    var prof = _orderService.GetProfitByCampaign(item.Id);
                    if (prof > 0)
                    {
                        balance = balance + prof;
                        campaignsInProfit.Add(item);
                    }
                }
            }

            if (unclProfits < 0)
                unclProfits = unclProfits * -1;

            if (balance < 0)
                balance = 0;

            unclProfits = unclProfits - balance;

            if (balance > 0)
            {
                var newCampaignInProfit = new List<CampaignRecord>();
                foreach (var camp in campaignsInProfit)
                {
                    //bool notInTranz = false;
                    //foreach (var tranz in list)
                    //{
                    //    if (!tranz.Event.StartsWith(camp.Alias) && tranz.Status == "Completed" && tranz.IsProfitPaid != null && tranz.IsProfitPaid == false && tranz.IsCampiaign)
                    //    {
                    //        notInTranz = true;
                    //    }
                    //}

                    if (list.Where(tranz => tranz.Event.StartsWith(camp.Alias)).ToList().Count == 0 || list.Count == 0)
                    {
                        var evant = T("{0} was delivered ({1} items sold)", camp.Alias, camp.ProductCountSold);
                        var payout = new PayoutRecord() { Date = DateTime.Now, Amount = _orderService.GetProfitByCampaign(camp.Id), Event = evant.ToString(), Currency_Id = _currencyRepository.Table.Where(c => c.Code == "RM").First().Id, IsPlus = true, UserId = currentUserId, Status = "Completed", IsCampiaign = true, IsProfitPaid = false };
                        _payoutService.AddPayout(payout);
                    }
                }
            }

            //list = null;
            model.Transactions = payouts.Where(t => t.UserId == currentUserId && t.IsOrder == false).Select(s => new History { Id = s.Id, Date = s.Date, Event = s.Event, Amount = s.Amount, IsPlus = s.IsPlus, UserId = s.UserId, Status = s.Status, CurrencyId = s.Currency_Id, Alias = string.Empty, CampaignName = string.Empty }).ToList();
            foreach (var camp in campaignsInProfit)
            {
                foreach (var tranz in model.Transactions)
                {
                    if (tranz.Event.StartsWith(camp.Alias))
                    {
                        tranz.Event = tranz.Event.Replace(camp.Alias, "");
                        tranz.Alias = camp.Alias;
                        tranz.CampaignName = camp.Title;
                    }
                    tranz.Currency = _currencyRepository.Table.Where(c => c.Id == tranz.CurrencyId).Select(c => c.Code).First();
                }
            }

            model.Balances.Where(b => b.Currency == "RM").First().UnclProfits = Math.Round(unclProfits, 2);
            model.Balances.Where(b => b.Currency == "RM").First().Bal = Math.Round(balance, 2);

            //model.Transactions = list;
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

            //var payouts = _payoutService.GetAllPayouts().ToList();
            double balance = 0;
            //foreach (var item in payouts)
            //{
            //    if (item.Currency_Id == currId)
            //    {
            //        if (item.IsPlus && item.UserId == currentUserId)
            //            balance = balance + item.Amount;
            //        else if (item.UserId == currentUserId)
            //            balance = balance - item.Amount;
            //    }
            //}

            var campaigns = _campaignService.GetCampaignsOfUser(currentUserId);
            foreach (var item in campaigns)
            {
                if (!item.IsActive && _orderService.IsOrdersForCampaignHasStatusDeliveredAndPaid(item.Id))
                {
                    balance = balance + _orderService.GetProfitByCampaign(item.Id);
                }
            }

            var pay = _payoutService.GetAllPayouts().Where(p => p.UserId == currentUserId && p.IsProfitPaid != null && p.IsProfitPaid == true && p.Status != "Pending").ToList();
            if (pay.Count > 0)
            {
                balance = balance - pay.Sum(p => p.Amount);
            }

            if (balance > 0)
            {
                var payout = new PayoutRecord() { Date = DateTime.Now, Amount = balance, Event = T("You requested a payout").ToString(), Currency_Id = currId, IsPlus = false, UserId = currentUserId, Status = "Pending", IsProfitPaid = false };
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

                _teeyootMessagingService.SendPayoutRequestMessageToAdmin(currentUserId, accountNumber, bankName, accHoldName, contNum, messAdmin);
                _teeyootMessagingService.SendPayoutRequestMessageToSeller(currentUserId, accountNumber, bankName, accHoldName, contNum);
            }
            
            return RedirectToAction("Accounts");
        }


    }
}