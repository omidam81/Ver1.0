using Braintree;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using Orchard.UI.Notify;
using RM.Localization.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;


namespace Teeyoot.Module.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;
        private readonly ICampaignService _campaignService;
        private readonly IDeliverySettingsService _deliverySettingService;
        private readonly INotifier _notifier;
        private readonly IimageHelper _imageHelper;
        private readonly IMailChimpSettingsService _settingsService;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IMessageService _messageService;
        private readonly IPaymentSettingsService _paymentSettingsService;
        private readonly IRepository<CommonSettingsRecord> _commonSettingsRepository;
        private readonly IRepository<OrderStatusRecord> _orderStatusRepository;

        private readonly IRepository<CheckoutCampaignRequest> _checkoutRequestRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<TeeyootUserPartRecord> _userRepository;
        private readonly IContentManager _contentManager;
        private string cultureUsed = string.Empty;
        private readonly ICookieCultureService _cookieCultureService;
        private readonly ICultureService _cultureService;



        public HomeController(IOrderService orderService,
                              ICampaignService campaignService,
                              INotifier notifier,
                              IPromotionService promotionService,
                              IimageHelper imageHelper,
                              IMailChimpSettingsService settingsService,
                              IPaymentSettingsService paymentSettingsService,
                              IShapeFactory shapeFactory,
                              ITeeyootMessagingService teeyootMessagingService,
                              IWorkContextAccessor workContextAccessor,
                              IRepository<TeeyootUserPartRecord> userRepository,
                              IDeliverySettingsService deliverySettingService,
                              IContentManager contentManager,
                              IRepository<CommonSettingsRecord> commonSettingsRepository,
                              IRepository<CheckoutCampaignRequest> checkoutRequestRepository,
                              ICookieCultureService cookieCultureService,
                              ICultureService cultureService,
                              IRepository<OrderStatusRecord> orderStatusRepository)
        {
            _orderService = orderService;
            _promotionService = promotionService;
            _campaignService = campaignService;
            _imageHelper = imageHelper;
            _deliverySettingService = deliverySettingService;
            _settingsService = settingsService;
            _teeyootMessagingService = teeyootMessagingService;
            _paymentSettingsService = paymentSettingsService;
            _commonSettingsRepository = commonSettingsRepository;
            _checkoutRequestRepository = checkoutRequestRepository;
            _workContextAccessor = workContextAccessor;
            _userRepository = userRepository;
            _contentManager = contentManager;
            _orderStatusRepository = orderStatusRepository;

            Logger = NullLogger.Instance;
            _notifier = notifier;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;

            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            _cookieCultureService = cookieCultureService;
            _cultureService = cultureService;
        }

        private ILogger Logger { get; set; }
        private Localizer T { get; set; }

        private dynamic Shape { get; set; }


        //public static BraintreeGateway Gateway = new BraintreeGateway
        //{
        //    Environment = Braintree.Environment.SANDBOX,
        //    PublicKey = "ny4y8s7fkcvnfw9t",
        //    PrivateKey = "1532863effa7197329266f7de4837bae",
        //    MerchantId = "7qw5pmrj3hqd2hr4"
        //};
        //
        // GET: /Home/
        public string Index()
        {
            return T("Welcome to Teeyoot!").ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult CreateOrder(IEnumerable<OrderProductViewModel> products)
        {
            if (products.Count() == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, T("Please, select at least one product to place your order").ToString());
            }

            try
            {
                var id = _orderService.CreateOrder(products).OrderPublicId;

                Response.Write(id);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                Logger.Error("Error occured when trying to create new order ---------------> " + e.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, T("Error occured when trying to create new order").ToString());
            }
        }


        [Themed]
        public ActionResult Payment(string orderId, string promo)
        {
            var order = _orderService.GetOrderByPublicId(orderId);

            if (order != null)
            {
                var campaignCulture = order.CurrencyRecord.CurrencyCulture;
                if (cultureUsed != campaignCulture)
                {
                    _cookieCultureService.SetCulture(campaignCulture);
                    return RedirectToAction("Payment", new { orderId = orderId, promo = promo });
                }
                var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == cultureUsed);

                var model = new PaymentViewModel();
                //model.CountryName = _cultureService.ListCultures().Where(c => c.Culture == cultureUsed).First().LocalizedName;
                string localName = _cultureService.ListCultures().Where(c => c.Culture == cultureUsed).First().LocalizedName;
                int firstIndex = localName.IndexOf("(") + 1;
                model.CountryName = localName.Substring(firstIndex, localName.IndexOf(")") - firstIndex);
                model.Order = order;
                //model.ClientToken = "eyJ2ZXJzaW9uIjoyLCJhdXRob3JpemF0aW9uRmluZ2VycHJpbnQiOiI1NGU1NmE0MmMwZTIzMGFiYjkyZjk2Njc4N2I3NDY4OTEzZDc5YmU5Zjg2NzE5NjI2N2FjMDMwYzEyZjk2ZTEyfGNyZWF0ZWRfYXQ9MjAxNS0wNy0wN1QwOToxNDoyOS41NTc5MDE5NDcrMDAwMFx1MDAyNm1lcmNoYW50X2lkPWRjcHNweTJicndkanIzcW5cdTAwMjZwdWJsaWNfa2V5PTl3d3J6cWszdnIzdDRuYzgiLCJjb25maWdVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvZGNwc3B5MmJyd2RqcjNxbi9jbGllbnRfYXBpL3YxL2NvbmZpZ3VyYXRpb24iLCJjaGFsbGVuZ2VzIjpbXSwiZW52aXJvbm1lbnQiOiJzYW5kYm94IiwiY2xpZW50QXBpVXJsIjoiaHR0cHM6Ly9hcGkuc2FuZGJveC5icmFpbnRyZWVnYXRld2F5LmNvbTo0NDMvbWVyY2hhbnRzL2RjcHNweTJicndkanIzcW4vY2xpZW50X2FwaSIsImFzc2V0c1VybCI6Imh0dHBzOi8vYXNzZXRzLmJyYWludHJlZWdhdGV3YXkuY29tIiwiYXV0aFVybCI6Imh0dHBzOi8vYXV0aC52ZW5tby5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tIiwiYW5hbHl0aWNzIjp7InVybCI6Imh0dHBzOi8vY2xpZW50LWFuYWx5dGljcy5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tIn0sInRocmVlRFNlY3VyZUVuYWJsZWQiOnRydWUsInRocmVlRFNlY3VyZSI6eyJsb29rdXBVcmwiOiJodHRwczovL2FwaS5zYW5kYm94LmJyYWludHJlZWdhdGV3YXkuY29tOjQ0My9tZXJjaGFudHMvZGNwc3B5MmJyd2RqcjNxbi90aHJlZV9kX3NlY3VyZS9sb29rdXAifSwicGF5cGFsRW5hYmxlZCI6dHJ1ZSwicGF5cGFsIjp7ImRpc3BsYXlOYW1lIjoiQWNtZSBXaWRnZXRzLCBMdGQuIChTYW5kYm94KSIsImNsaWVudElkIjpudWxsLCJwcml2YWN5VXJsIjoiaHR0cDovL2V4YW1wbGUuY29tL3BwIiwidXNlckFncmVlbWVudFVybCI6Imh0dHA6Ly9leGFtcGxlLmNvbS90b3MiLCJiYXNlVXJsIjoiaHR0cHM6Ly9hc3NldHMuYnJhaW50cmVlZ2F0ZXdheS5jb20iLCJhc3NldHNVcmwiOiJodHRwczovL2NoZWNrb3V0LnBheXBhbC5jb20iLCJkaXJlY3RCYXNlVXJsIjpudWxsLCJhbGxvd0h0dHAiOnRydWUsImVudmlyb25tZW50Tm9OZXR3b3JrIjp0cnVlLCJlbnZpcm9ubWVudCI6Im9mZmxpbmUiLCJ1bnZldHRlZE1lcmNoYW50IjpmYWxzZSwiYnJhaW50cmVlQ2xpZW50SWQiOiJtYXN0ZXJjbGllbnQzIiwibWVyY2hhbnRBY2NvdW50SWQiOiJzdGNoMm5mZGZ3c3p5dHc1IiwiY3VycmVuY3lJc29Db2RlIjoiVVNEIn0sImNvaW5iYXNlRW5hYmxlZCI6dHJ1ZSwiY29pbmJhc2UiOnsiY2xpZW50SWQiOiIxMWQyNzIyOWJhNThiNTZkN2UzYzAxYTA1MjdmNGQ1YjQ0NmQ0ZjY4NDgxN2NiNjIzZDI1NWI1NzNhZGRjNTliIiwibWVyY2hhbnRBY2NvdW50IjoiY29pbmJhc2UtZGV2ZWxvcG1lbnQtbWVyY2hhbnRAZ2V0YnJhaW50cmVlLmNvbSIsInNjb3BlcyI6ImF1dGhvcml6YXRpb25zOmJyYWludHJlZSB1c2VyIiwicmVkaXJlY3RVcmwiOiJodHRwczovL2Fzc2V0cy5icmFpbnRyZWVnYXRld2F5LmNvbS9jb2luYmFzZS9vYXV0aC9yZWRpcmVjdC1sYW5kaW5nLmh0bWwiLCJlbnZpcm9ubWVudCI6Im1vY2sifSwibWVyY2hhbnRJZCI6ImRjcHNweTJicndkanIzcW4iLCJ2ZW5tbyI6Im9mZmxpbmUiLCJhcHBsZVBheSI6eyJzdGF0dXMiOiJtb2NrIiwiY291bnRyeUNvZGUiOiJVUyIsImN1cnJlbmN5Q29kZSI6IlVTRCIsIm1lcmNoYW50SWRlbnRpZmllciI6Im1lcmNoYW50LmNvbS5icmFpbnRyZWVwYXltZW50cy5zYW5kYm94LkJyYWludHJlZS1EZW1vIiwic3VwcG9ydGVkTmV0d29ya3MiOlsidmlzYSIsIm1hc3RlcmNhcmQiLCJhbWV4Il19fQ==";
                model.ClientToken = setting.ClientToken;
                //var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == DEFAULT_LANGUAGE_CODE);

                model.CashDeliv = setting.CashDeliv;
                model.CreditCard = setting.CreditCard;
                model.Mol = setting.Mol;
                model.PayPal = setting.PayPal;

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



        private string GetVCode(string input)
        {
            var result = "";
            var md5 = MD5.Create();
            var md5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var stringBuilder = new StringBuilder();
            foreach (byte data in md5Bytes)
            {
                stringBuilder.Append(data.ToString("x2"));
            }
            result = stringBuilder.ToString();

            return result;
        }

        public string Molpay(OrderRecord order, string country, string firstName, string lastName, string email, string state, string phone, double deliveryCost)
        {

            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == cultureUsed);

            //var merchantId = "teeyoot1_Dev";
            //var verifyKey = "856287426298f7e8508eae9896c09c03";
            var merchantId = setting.MerchantIdMol;
            var verifyKey = setting.VerifyKey;


            //var Total = order.TotalPrice;
            var Total = ((order.TotalPriceWithPromo != 0 ? order.TotalPriceWithPromo : order.TotalPrice) + deliveryCost).ToString("F2", CultureInfo.InvariantCulture);
            var OrderNumber = order.Id;

            var Campaign = _campaignService.GetCampaignById(order.Products.First().CampaignProductRecord.CampaignRecord_Id);
            var title = Campaign.Title;
            var campId = Campaign.Id;
            var name = firstName + " " + lastName;
            var Email = email;
            var Phone = phone;
            var description = campId + ", " + title + "," + "\nOrdered from Teeyoot " + country + " " + state;



            var vCode = GetVCode(Total + merchantId + OrderNumber + verifyKey);
            //var paymentUrl = "";
            //if (method == "credit")
            //    {
            //        paymentUrl = "https://www.onlinepayment.com.my/MOLPay/pay/" + merchantId + "?amount=" +
            //                  Total + "&orderid=" + OrderNumber +
            //                  "&bill_name=" + Name + "&channel=credit&bill_email=" + Email + "&bill_mobile=" + Phone +
            //                  "&bill_desc=" + Description + "&vcode=" + vCode;

            //    } else {
            var paymentUrl = "https://www.onlinepayment.com.my/MOLPay/pay/" + merchantId + "?amount=" +
                              Total + "&orderid=" + OrderNumber +
                              "&bill_name=" + name + "&bill_email=" + Email + "&bill_mobile=" + Phone +
                              "&bill_desc=" + description + "&vcode=" + vCode;

            //}


            return paymentUrl;
        }


        public ActionResult Molpas()
        {
            var merchantId = "7qw5pmrj3hqd2hr4";
            var total = "51.99";
            var orderNumber = 352;
            var verifyKey = "856287426298f7e8508eae9896c09c03";
            var vCode = GetVCode(total + merchantId + orderNumber + verifyKey);
            var model = new MolpasViewModel() { vcode = vCode };

            return View(model);
        }

        //[HttpGet]
        public ActionResult CallbackMolpay(string amount, string orderid, string appcode, string tranID, string domain, string status, string error_code,
                                    string error_desc, string currency, string paydate, string channel, string skey)
        {

            string destFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/molPayLog");
            var dir = new DirectoryInfo(destFolder);

            if (!dir.Exists)
            {
                Directory.CreateDirectory(destFolder);
            }
            var request = System.Web.HttpContext.Current.Request;
            System.IO.File.AppendAllText(destFolder + "/mol.txt", DateTime.Now + "  -------------  " + "Return Url status:" + status + "; amount: " + amount + "; orderid: " + orderid + "; error_desc: " + error_desc + "          " + request.Url + "\r\n" + "tranId: " + tranID +
                                         " domain: " + domain + " error_code: " + error_code + "; skey: " + skey + "; channel: " + channel + "\r\n");

            if (orderid == null)
                return View();


                var order = _orderService.GetOrderById(Convert.ToInt32(orderid));
                var campaign = _campaignService.GetCampaignById(order.Products.First().CampaignProductRecord.CampaignRecord_Id);

                //if (order.OrderStatusRecord.Name == OrderStatus.Unapproved.ToString())
                //{


                if (status == "00")
                {

                    //order.OrderStatusRecord = _orderStatusRepository.Get(int.Parse(OrderStatus.Approved.ToString("d")));
                    order.OrderStatusRecord = _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Approved.ToString());
                    _orderService.UpdateOrder(order);

                    if (campaign.ProductCountSold > campaign.ProductCountGoal)
                    {
                        campaign.ProductCountSold += order.Products.Sum(p => (int?)p.Count) ?? 0;
                        _campaignService.UpdateCampaign(campaign);
                    }
                    else
                    {
                        campaign.ProductCountSold += order.Products.Sum(p => (int?)p.Count) ?? 0;
                        _campaignService.UpdateCampaign(campaign);
                        if (campaign.ProductCountSold > campaign.ProductCountGoal)
                        {
                            _teeyootMessagingService.SendCampaignMetMinimumMessageToBuyers(campaign.Id);
                            _teeyootMessagingService.SendCampaignMetMinimumMessageToSeller(campaign.Id);
                        }
                    }


                    var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                    var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                    var users = _userRepository.Table.ToList();
                //_teeyootMessagingService.SendNewOrderMessageToAdmin(order.Id, pathToMedia, pathToTemplates);
                    _teeyootMessagingService.SendOrderStatusMessage(pathToTemplates, pathToMedia, order.Id, OrderStatus.Approved.ToString());

                    var commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).FirstOrDefault();
                    if (commonSettings == null)
                    {
                        _commonSettingsRepository.Create(new CommonSettingsRecord() { DoNotAcceptAnyNewCampaigns = false, CommonCulture = cultureUsed });
                        commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).First();
                    }

                }
                else if (status == "11")
                {
                    return RedirectToAction("ReservationComplete", new { campaignId = campaign.Id, sellerId = campaign.TeeyootUserId, oops = true });
                }

                return RedirectToAction("ReservationComplete", new { campaignId = campaign.Id, sellerId = campaign.TeeyootUserId });
           

        }

        public JsonResult GetSettings()
        {
            var settings = _deliverySettingService.GetAllSettings().ToArray();
            return Json(new
            {
                settings = settings
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTransaction(FormCollection collection)
        {
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');

            var setting = _paymentSettingsService.GetAllSettigns().FirstOrDefault(s => s.Culture == cultureUsed);

            BraintreeGateway Gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                PublicKey = setting.PublicKey,
                PrivateKey = setting.PrivateKey,
                MerchantId = setting.MerchantId
            };



            Result<Transaction> result;
            var nonceId = Request.Form["payment_method_nonce"];
            if (collection["paumentMeth"] == "2")
            {
                TransactionRequest requestPayPal = new TransactionRequest
                {
                    Amount = 1000.0M,
                    PaymentMethodNonce = "fake-valid-nonce",
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = false,
                        StoreInVault = true
                    }
                };

                result = Gateway.Transaction.Sale(requestPayPal);
            }
            else
                if (collection["paumentMeth"] == "1")
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
                            StoreInVault = true,
                            SubmitForSettlement = false
                        }
                    };

                    result = Gateway.Transaction.Sale(requestCard);

                }
                else if (collection["paumentMeth"] == "4")
                {
                    //Отправляем сообщение админу
                }
                else if (collection["paumentMeth"] == "3")
                {
                    var method = collection["Bank"];

                    var meth1 = collection["paumentMeth"];
                    int id1 = int.Parse(collection["OrderId"]);
                    var orderMol = _orderService.GetOrderById(id1);
                    var campId = orderMol.Products.First().CampaignProductRecord.CampaignRecord_Id;
                    orderMol.Email = collection["Email"];
                    orderMol.FirstName = collection["FirstName"];
                    orderMol.LastName = collection["LastName"];
                    orderMol.StreetAddress = collection["StreetAddress"] + " " + collection["StreetAddress2"];
                    orderMol.City = collection["City"];
                    orderMol.State = collection["State"];
                    orderMol.PostalCode = collection["PostalCode"];
                    orderMol.Country = collection["Country"];
                    orderMol.PhoneNumber = collection["PhoneNumber"];
                    orderMol.Reserved = DateTime.UtcNow;
                    orderMol.OrderStatusRecord = _orderStatusRepository.Get(int.Parse(OrderStatus.Unapproved.ToString("d")));

                    orderMol.Delivery = _deliverySettingService.GetAllSettings()
                        .FirstOrDefault(s => s.State == collection["State"])
                        .DeliveryCost;

                    /*
                    if (orderMol.TotalPriceWithPromo > 0)
                    {
                        orderMol.TotalPriceWithPromo = orderMol.TotalPriceWithPromo + _deliverySettingService.GetAllSettings().FirstOrDefault(s => s.State == collection["State"]).DeliveryCost;
                    }
                    orderMol.TotalPrice = orderMol.TotalPrice + _deliverySettingService.GetAllSettings().FirstOrDefault(s => s.State == collection["State"]).DeliveryCost - orderMol.Promotion;
                    */
                    orderMol.IsActive = true;
                    if (collection["PromoId"] != null)
                    {
                        PromotionRecord promotion = _promotionService.GetPromotionByPromoId(collection["PromoId"]);
                        promotion.Redeemed = promotion.Redeemed + 1;
                    }
                  
                    _teeyootMessagingService.SendNewOrderMessageToAdmin(orderMol.Id, pathToMedia, pathToTemplates);
                    //_teeyootMessagingService.SendNewOrderMessageToBuyer(orderMol.Id, pathToMedia, pathToTemplates);

                    var url = Molpay(_orderService.GetOrderById(int.Parse(collection["OrderId"])), collection["Country"], collection["FirstName"], collection["LastName"], collection["Email"], collection["State"], collection["PhoneNumber"], _deliverySettingService.GetAllSettings().FirstOrDefault(s => s.State == collection["State"]).DeliveryCost);
                    return Redirect(url);
                }




            var meth = collection["paumentMeth"];
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

            order.Delivery = _deliverySettingService.GetAllSettings()
                .FirstOrDefault(s => s.State == collection["State"])
                .DeliveryCost;

            /*
            if (order.TotalPriceWithPromo > 0)
            {
                order.TotalPriceWithPromo = order.TotalPriceWithPromo + _deliverySettingService.GetAllSettings().FirstOrDefault(s => s.State == collection["State"]).DeliveryCost;
            }
             */
            order.OrderStatusRecord = _orderStatusRepository.Table.First(s => s.Name == OrderStatus.Approved.ToString());
            /*
            order.TotalPrice = order.TotalPrice + _deliverySettingService.GetAllSettings().FirstOrDefault(s => s.State == collection["State"]).DeliveryCost - order.Promotion;
             */
            order.IsActive = true;





            var campaign = _campaignService.GetCampaignById(campaignId);
            if (campaign.ProductCountSold > campaign.ProductCountGoal)
            {
                campaign.ProductCountSold += order.Products.Sum(p => (int?)p.Count) ?? 0;
                _campaignService.UpdateCampaign(campaign);
            }
            else
            {
                campaign.ProductCountSold += order.Products.Sum(p => (int?)p.Count) ?? 0;
                _campaignService.UpdateCampaign(campaign);
                if (campaign.ProductCountSold > campaign.ProductCountGoal)
                {
                    _teeyootMessagingService.SendCampaignMetMinimumMessageToBuyers(campaign.Id);
                    _teeyootMessagingService.SendCampaignMetMinimumMessageToSeller(campaign.Id);
                }
            }



            //_orderService.UpdateOrder(order, OrderStatus.Approved);
            if (collection["PromoId"] != null)
            {
                PromotionRecord promotion = _promotionService.GetPromotionByPromoId(collection["PromoId"]);
                promotion.Redeemed = promotion.Redeemed + 1;
            }
            //Transaction transaction = result.Target;
            //ViewData["TransactionId"] = transaction.Id;
            //_notifier.Information(T("The transaction is successful"));
               
            var users = _userRepository.Table.ToList();
            _teeyootMessagingService.SendNewOrderMessageToAdmin(order.Id, pathToMedia, pathToTemplates);
            _teeyootMessagingService.SendOrderStatusMessage(pathToTemplates, pathToMedia, order.Id, OrderStatus.Approved.ToString());
               
            var commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).FirstOrDefault();
            if (commonSettings == null)
            {
                _commonSettingsRepository.Create(new CommonSettingsRecord() { DoNotAcceptAnyNewCampaigns = false, CommonCulture = cultureUsed });
                commonSettings = _commonSettingsRepository.Table.Where(s => s.CommonCulture == cultureUsed).First();
            }
            if (commonSettings.DoNotAcceptAnyNewCampaigns)
            {
                var request = new CheckoutCampaignRequest
                {
                    RequestUtcDate = DateTime.UtcNow,
                    Email = order.Email
                };
                _checkoutRequestRepository.Create(request);
            }




            return RedirectToAction("ReservationComplete", new { campaignId = campaign.Id, sellerId = campaign.TeeyootUserId });
            //}
            //else
            //{
            //    _notifier.Information(T("The transaction is failed"));
            //    return RedirectToAction("Payment", new { orderId = collection["OrderPublicId"], promo = collection["PromoId"] });
            //}          
        }

        [Themed]
        public ActionResult ReservationComplete(int campaignId, int sellerId, bool oops = false)
        {
            var campaigns = _campaignService.GetAllCampaigns()
                                .Where(c => c.TeeyootUserId == sellerId && c.IsApproved && c.Id != campaignId && c.CampaignCulture == cultureUsed)
                                .Select(c => new
                                {
                                    Id = c.Id,
                                    Alias = c.Alias,
                                    Title = c.Title,
                                    Goal = c.ProductMinimumGoal,
                                    Sold = c.ProductCountSold,
                                    ShowBack = c.BackSideByDefault,
                                    EndDate = c.EndDate
                                })
                                .ToArray();

            var entriesProjection = campaigns.Select(e =>
            {
                return Shape.campaign(
                    Id: e.Id,
                    Title: e.Title,
                    Sold: e.Sold,
                    Goal: e.Goal,
                    ShowBack: e.ShowBack,
                    Alias: e.Alias,
                    EndDate: e.EndDate,
                    FirstProductId: _campaignService.GetAllCampaignProducts().First(p => p.CampaignRecord_Id == e.Id && p.WhenDeleted== null).Id
                    );
            });

            var model = new ReservationCompleteViewModel();
            if (!oops)
                model.Message = T("Your reservation is confirmed. We will notify you once the T-shirt is ready. Meanwhile check out other designs or campaigns from the same seller").ToString();
            else
                model.Message = T("Oops! we couldn't process this order, you can try to change your payment method and try again").ToString();

            model.Campaigns = entriesProjection.ToArray();

            return View(model);
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

            if (order.OrderStatusRecord.Name == OrderStatus.Unapproved.ToString())
            {
                _notifier.Error(T("Your order has not been yet approved"));
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
            model.CultureInfo = CultureInfo.GetCultureInfo(_workContextAccessor.GetContext().CurrentCulture);
            model.CreateDate = order.Created.ToLocalTime().ToString("dd MMM HH:mm", model.CultureInfo);
            var campaign = _campaignService.GetCampaignById(order.Products[0].CampaignProductRecord.CampaignRecord_Id);
            model.CampaignName = campaign.Title;
            model.CampaignAlias = campaign.Alias;
            model.TotalPrice = (order.TotalPrice + order.Delivery).ToString();

            return View(model);
        }

        [Themed]
        public ActionResult RecoverOrder(string email)
        {
            if (email == null)
            {
                return View("RecoverOrder");
            }
            var orders = _orderService.GetActiveOrdersByEmailForLastTwoMoth(email);

            if (orders.Count() == 0)
            {
                var infoMessage = T("No orders found during last 60 days");
                _notifier.Add(NotifyType.Information, infoMessage);
            }
            else
            {
                var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                _teeyootMessagingService.SendRecoverOrderMessage(pathToTemplates, orders.ToList(), email);
                var infoMessage = T("Success! We have sent an email to " + email + " detailing your orders during the past 60 days.");
                _notifier.Add(NotifyType.Information, infoMessage);
            }
            return View("RecoverOrder");
        }

        public ActionResult CancelOrder(int orderId, string publicId)
        {
            try
            {
                var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
                var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                _teeyootMessagingService.SendOrderStatusMessage(pathToTemplates, pathToMedia, orderId, "Cancelled");
                _orderService.DeleteOrder(orderId);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                Logger.Error("Error occured when trying to delete an order ---------------> " + ex.ToString());
                return RedirectToAction("OrderTracking", new { orderId = publicId });
            }
        }


        [Themed]
        [HttpPost]
        public HttpStatusCodeResult ShareCampaign(bool isBack, int campaignId)
        {
            CampaignRecord campaign = _campaignService.GetCampaignById(campaignId);
            int product = _campaignService.GetProductsOfCampaign(campaignId).Where(pr=>pr.WhenDeleted == null).First().Id;

            string destFolder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), product.ToString(), "social");
            var dir = new DirectoryInfo(destFolder);

            if (dir.Exists == false || ((dir.Exists == true) && (dir.GetFiles().Count() == 0)))
            {
                try
                {
                    Directory.CreateDirectory(destFolder);

                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;
                    DesignInfo data = serializer.Deserialize<DesignInfo>(campaign.Design);

                    var p = campaign.Products.Where(pr=>pr.WhenDeleted ==null).First();

                    var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                    var rgba = ColorTranslator.FromHtml(p.ProductColorRecord.Value);

                    if (!isBack)
                    {
                        var frontPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_front.png");
                        var imgPath = new Bitmap(frontPath);

                        _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Front);
                    }
                    else
                    {
                        var backPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_back.png");
                        var imgPath = new Bitmap(backPath);

                        _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Back);
                    }
                }
                catch
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        [HttpPost]
        public HttpStatusCodeResult ReservCampaign(string email, int id)
        {
            var requests = _campaignService.GetReservedRequestsOfCampaign(id);

            foreach (var request in requests)
            {
                if (request.Email == email)
                {
                    Response.Write("Already");
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }

            _campaignService.ReservCampaign(id, email);

            if (requests.Count() >= 10)
            {
                _teeyootMessagingService.SendReLaunchCampaignMessageToAdmin(id);
                _teeyootMessagingService.SendReLaunchCampaignMessageToSeller(id);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}
