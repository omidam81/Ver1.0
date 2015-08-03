using Braintree;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;
        private readonly ICampaignService _campaignService;
        private readonly INotifier _notifier;

        public HomeController(IOrderService orderService, ICampaignService campaignService, INotifier notifier, IPromotionService promotionService)
        {
            _orderService = orderService;
            _promotionService = promotionService;
            _campaignService = campaignService;

            Logger = NullLogger.Instance;
            _notifier = notifier;
            T = NullLocalizer.Instance;
        }

        private ILogger Logger { get; set; }
        private Localizer T { get; set; }

        public static BraintreeGateway Gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            PublicKey = "ny4y8s7fkcvnfw9t",
            PrivateKey = "1532863effa7197329266f7de4837bae",
            MerchantId = "7qw5pmrj3hqd2hr4"
        };
        //
        // GET: /Home/
        public string Index()
        {
            return "Welcome to Teeyoot!";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTransaction(FormCollection collection)
        {
            string res = "";
            Result<Transaction> result;
            var nonceId = Request.Form["payment_method_nonce"];
            if (Request.Form["payment_method_nonce"] != "")
            {
                TransactionRequest requestPayPal = new TransactionRequest
                {
                    Amount = 1000.0M,
                    PaymentMethodNonce = "fake-valid-nonce",
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true,
                        StoreInVault = true
                    }
                };

                result = Gateway.Transaction.Sale(requestPayPal);
            }
            else
            {
                TransactionRequest requestCard = new TransactionRequest
                {
                    Amount = 1000.0M, //Здесь указывается сумма транзакции в USD
                    CreditCard = new TransactionCreditCardRequest
                    {
                        Number = collection["number"],
                        CVV = collection["cvv"],
                        ExpirationMonth = collection["month"],
                        ExpirationYear = collection["year"]
                    },
                    Options = new TransactionOptionsRequest
                    {
                        StoreInVault = true
                    }
                };
                result = Gateway.Transaction.Sale(requestCard);
                //result = Gateway.Transaction.SubmitForSettlement("the_transaction_id", 1000.0M);
            }

            if (result.IsSuccess())
            {
                int id = int.Parse(collection["OrderId"]);
                var order = _orderService.GetOrderById(id);
                var campaignId = order.Products.First().CampaignProductRecord.CampaignRecord_Id;
                order.Email = collection["Email"];
                order.FirstName = collection["FirstName"];
                order.LastName = collection["LastName"];
                order.StreetAddress = collection["StreetAddress"] + " " + collection["StreetAddress2"];
                order.City = collection["City"];
                order.State = collection["State"];
                order.PostalCode = collection["PostalCode"];
                order.Country = collection["Country"];
                order.PhoneNumber = collection["PhoneNumber"];
                order.Reserved = DateTime.UtcNow;
                order.IsActive = true;

                _orderService.UpdateOrder(order, OrderStatus.Reserved);
                if (collection["PromoId"] != null)
                {
                    PromotionRecord promotion = _promotionService.GetPromotionByPromoId(collection["PromoId"]);
                    promotion.Redeemed = promotion.Redeemed + 1;
                }
                var campaign = _campaignService.GetCampaignById(campaignId);
                campaign.ProductCountSold += order.Products.Sum(p => (int?)p.Count) ?? 0;
                _campaignService.UpdateCampaign(campaign);

                Transaction transaction = result.Target;
                ViewData["TransactionId"] = transaction.Id;
                res = "The transaction is successful";
            }
            else
            {
                res = "The transaction is failed";
                ViewData["Message"] = result.Message;
            }

            return RedirectToAction("Payment", new { orderId = collection["OrderPublicId"], promo = collection["PromoId"], result = res });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult CreateOrder(IEnumerable<OrderProductViewModel> products)
        {
            try
            {
                var id = _orderService.CreateOrder(products).OrderPublicId;

                Response.Write(id);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error("Error occured when trying to create new order ---------------> " + e.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error occured when trying to create new order");
            }
        }


        [Themed]
        public ActionResult Payment(string orderId, string promo, string result = "")
        {
            var order = _orderService.GetOrderByPublicId(orderId);

            if (order != null)
            {
                var model = new PaymentViewModel();
                model.Order = order;
                model.Result = result;
                model.ClientToken = "eyJ2ZXJzaW9uIjoyLCJhdXRob3JpemF0aW9uRmluZ2VycHJpbnQiOiI1NGU1NmE0MmMwZTIzMGFiYjkyZjk2Njc4N2I3NDY4OTEzZDc5YmU5Zjg2NzE5NjI2N2FjMDMwYzEyZjk2ZTEyfGNyZWF0ZWRfYXQ9MjAxNS0wNy0wN1QwOToxNDoyOS41NTc5MDE5NDcrMDAwMFx1MDAyNm1lcmNoYW50X2lkPWRjcHNweTJicndkanIzcW5cdTAwMjZwdWJsaWNfa2V5PTl3d3J6cWszdnIzdDRuYzgiLCJjb25maWdVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvZGNwc3B5MmJyd2RqcjNxbi9jbGllbnRfYXBpL3YxL2NvbmZpZ3VyYXRpb24iLCJjaGFsbGVuZ2VzIjpbXSwiZW52aXJvbm1lbnQiOiJzYW5kYm94IiwiY2xpZW50QXBpVXJsIjoiaHR0cHM6Ly9hcGkuc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbTo0NDMvbWVyY2hhbnRzL2RjcHNweTJicndkanIzcW4vY2xpZW50X2FwaSIsImFzc2V0c1VybCI6Imh0dHBzOi8vYXNzZXRzLmJyYWludHJlZWdhdGV3YXkuY29tIiwiYXV0aFVybCI6Imh0dHBzOi8vYXV0aC52ZW5tby5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tIiwiYW5hbHl0aWNzIjp7InVybCI6Imh0dHBzOi8vY2xpZW50LWFuYWx5dGljcy5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tIn0sInRocmVlRFNlY3VyZUVuYWJsZWQiOnRydWUsInRocmVlRFNlY3VyZSI6eyJsb29rdXBVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvZGNwc3B5MmJyd2RqcjNxbi90aHJlZV9kX3NlY3VyZS9sb29rdXAifSwicGF5cGFsRW5hYmxlZCI6dHJ1ZSwicGF5cGFsIjp7ImRpc3BsYXlOYW1lIjoiQWNtZSBXaWRnZXRzLCBMdGQuIChTYW5kYm94KSIsImNsaWVudElkIjpudWxsLCJwcml2YWN5VXJsIjoiaHR0cDovL2V4YW1wbGUuY29tL3BwIiwidXNlckFncmVlbWVudFVybCI6Imh0dHA6Ly9leGFtcGxlLmNvbS90b3MiLCJiYXNlVXJsIjoiaHR0cHM6Ly9hc3NldHMuYnJhaW50cmVlZ2F0ZXdheS5jb20iLCJhc3NldHNVcmwiOiJodHRwczovL2NoZWNrb3V0LnBheXBhbC5jb20iLCJkaXJlY3RCYXNlVXJsIjpudWxsLCJhbGxvd0h0dHAiOnRydWUsImVudmlyb25tZW50Tm9OZXR3b3JrIjp0cnVlLCJlbnZpcm9ubWVudCI6Im9mZmxpbmUiLCJ1bnZldHRlZE1lcmNoYW50IjpmYWxzZSwiYnJhaW50cmVlQ2xpZW50SWQiOiJtYXN0ZXJjbGllbnQzIiwibWVyY2hhbnRBY2NvdW50SWQiOiJzdGNoMm5mZGZ3c3p5dHc1IiwiY3VycmVuY3lJc29Db2RlIjoiVVNEIn0sImNvaW5iYXNlRW5hYmxlZCI6dHJ1ZSwiY29pbmJhc2UiOnsiY2xpZW50SWQiOiIxMWQyNzIyOWJhNThiNTZkN2UzYzAxYTA1MjdmNGQ1YjQ0NmQ0ZjY4NDgxN2NiNjIzZDI1NWI1NzNhZGRjNTliIiwibWVyY2hhbnRBY2NvdW50IjoiY29pbmJhc2UtZGV2ZWxvcG1lbnQtbWVyY2hhbnRAZ2V0YnJhaW50cmVlLmNvbSIsInNjb3BlcyI6ImF1dGhvcml6YXRpb25zOmJyYWludHJlZSB1c2VyIiwicmVkaXJlY3RVcmwiOiJodHRwczovL2Fzc2V0cy5icmFpbnRyZWVnYXRld2F5LmNvbS9jb2luYmFzZS9vYXV0aC9yZWRpcmVjdC1sYW5kaW5nLmh0bWwiLCJlbnZpcm9ubWVudCI6Im1vY2sifSwibWVyY2hhbnRJZCI6ImRjcHNweTJicndkanIzcW4iLCJ2ZW5tbyI6Im9mZmxpbmUiLCJhcHBsZVBheSI6eyJzdGF0dXMiOiJtb2NrIiwiY291bnRyeUNvZGUiOiJVUyIsImN1cnJlbmN5Q29kZSI6IlVTRCIsIm1lcmNoYW50SWRlbnRpZmllciI6Im1lcmNoYW50LmNvbS5icmFpbnRyZWVwYXltZW50cy5zYW5kYm94LkJyYWludHJlZS1EZW1vIiwic3VwcG9ydGVkTmV0d29ya3MiOlsidmlzYSIsIm1hc3RlcmNhcmQiLCJhbWV4Il19fQ==";
                if (promo != null)
                {
                    PromotionRecord promotion = _promotionService.GetPromotionByPromoId(promo);
                    model.Promotion = promotion;
                    if (promotion.AmountType == "%")
                    {
                        model.Order.Promotion = (model.Order.TotalPrice / 100) * promotion.AmountSize;
                        model.Order.TotalPriceWithPromo = model.Order.TotalPrice - model.Order.Promotion;
                    }
                    else
                    {
                        if (promotion.AmountType == order.CurrencyRecord.Code)
                        {
                            model.Order.Promotion = promotion.AmountSize;
                            model.Order.TotalPriceWithPromo = model.Order.TotalPrice - model.Order.Promotion;
                        }
                    }
                }
                return View(model);
            }
            else
            {
                return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
            }
        }


        [Themed]
        public ActionResult TrackOrder()
        {

            var message = TempData["OrderNotFoundMessage"];
            if (message != null && !string.IsNullOrWhiteSpace(message.ToString()))
                _notifier.Error(T(message.ToString()));
            return View();
        }

        [HttpPost]
        public ActionResult SearchForOrder(string orderId)
        {
            return RedirectToAction("OrderTracking", new { orderId });
        }

        [Themed]
        [HttpGet]
        public ActionResult OrderTracking(string orderId)
        {
            var order = _orderService.GetActiveOrderByPublicId(orderId);

            if (order == null)
            {
                _notifier.Error(T("Could not find order with that lookup number"));
                return View("TrackOrder");
            }

            var model = new OrderTrackingViewModel();
            model.OrderId = order.Id;
            model.OrderPublicId = orderId;
            model.Status = order.OrderStatusRecord;
            model.Products = order.Products.ToArray();
            model.ShippingTo = new string[] {
                order.FirstName + " " + order.LastName,
                order.StreetAddress,
                order.City + ", " + order.State + ", " + order.Country + " " + order.PostalCode
            };
            model.Events = order.Events.ToArray();
            // TODO: eugene: get culture if needed
            model.CreateDate = order.Created.ToLocalTime().ToString("dd MMM HH:mm", CultureInfo.GetCultureInfo("en-US"));
            var campaign = _campaignService.GetCampaignById(order.Products[0].CampaignProductRecord.CampaignRecord_Id);
            model.CampaignName = campaign.Title;
            model.CampaignAlias = campaign.Alias;

            return View(model);
        }

        public ActionResult CancelOrder(int orderId, string publicId)
        {
            try
            {
                _orderService.DeleteOrder(orderId);
                //TODO: eugene: mail customer if the order was cancelled
                return Redirect("/");
            }
            catch(Exception ex)
            {
                Logger.Error("Error occured when trying to delete an order ---------------> " + ex.ToString());
                return RedirectToAction("OrderTracking", new { orderId = publicId });
            }
        }
    }
}