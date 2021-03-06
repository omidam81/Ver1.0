﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Orders.Controllers
{
    [Admin]
    public class HomeController : Controller
    {
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly IRepository<OrderStatusRecord> _orderStatusRepository;
        private readonly IOrderService _orderService;
        private readonly ICampaignService _campaignService;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IPayoutService _payoutService;
        private readonly INotifier _notifierService;
        private readonly ITeeyootMessagingService _teeyootMessagingService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ILogger Logger { get; set; }
        private dynamic Shape { get; set; }
        // GET: Home

        public HomeController(
            IRepository<CurrencyRecord> currencyRepository,
            IRepository<OrderStatusRecord> orderStatusRepository,
            IOrderService orderService,
            ICampaignService campaignService,
            IShapeFactory shapeFactory,
            IContentManager contentManager,
            ISiteService siteService,
            IPayoutService payoutService,
            INotifier notifierService,
            ITeeyootMessagingService teeyootMessagingService,
            IWorkContextAccessor workContextAccessor)
        {
            _currencyRepository = currencyRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderService = orderService;
            _campaignService = campaignService;
            _contentManager = contentManager;
            _siteService = siteService;
            _payoutService = payoutService;
            _notifierService = notifierService;
            _teeyootMessagingService = teeyootMessagingService;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
            _workContextAccessor = workContextAccessor;
        }

        public Localizer T { get; set; }

        public ActionResult Index(int? filterCurrencyId, PagerParameters pagerParameters)
        {
            var orders = _orderService.GetAllOrders()
                .Where(o => o.IsActive);

            if (filterCurrencyId.HasValue)
            {
                var filterCurrency = _currencyRepository.Get(filterCurrencyId.Value);
                orders = orders.Where(o => o.CurrencyRecord == filterCurrency);
            }

            var orderEntities = new AdminOrderViewModel();

            foreach (var item in orders)
            {
                try
                {
                    var it = item.Products.First().CampaignProductRecord.CampaignRecord_Id;
                }
                catch (Exception)
                {
                    continue;
                }

                var campaignId = item.Products.First().CampaignProductRecord.CampaignRecord_Id;
                if (campaignId == null)
                    continue;

                var campaign = _campaignService.GetCampaignById(campaignId);

                try
                {

                    var seller = _contentManager.Query<UserPart, UserPartRecord>()
                        .Where(user => user.Id == campaign.TeeyootUserId)
                        .List()
                        .First();

                    double orderProfit = 0;

                    foreach (var product in item.Products)
                    {
                        var prof = product.CampaignProductRecord.Price - product.CampaignProductRecord.BaseCost;
                        foreach (var size in product.CampaignProductRecord.ProductRecord.SizesAvailable)
                        {
                            if (size.Id == product.ProductSizeRecord.Id)
                                prof = prof - size.SizeCost;
                        }
                        orderProfit = orderProfit + (prof*product.Count);
                        orderProfit = Math.Round(orderProfit, 2);
                    }


                    //if (string.IsNullOrWhiteSpace(searchString) || campaign.Title.ToLower().Contains(searchString.ToLower()))
                    //{
                    orderEntities.Orders.Add(new AdminOrder
                    {
                        PublicId = item.OrderPublicId,
                        Products = item.Products,
                        Status = item.OrderStatusRecord.Name,
                        EmailBuyer = item.Email,
                        CampaignId = campaign.Id,
                        CampaignName = campaign.Title,
                        CampaignAlias = campaign.Alias,
                        Id = item.Id,
                        Profit = orderProfit,
                        SellerId = seller != null ? seller.Id : 0,
                        Payout = item.ProfitPaid,
                        CreateDate = item.Created,
                        UserNameSeller = seller != null ? seller.UserName : "",
                        Currency = item.CurrencyRecord.ShortName
                    });
                    //}
                }
                catch (Exception ex)
                {

                    Logger.Error(ex, campaign.TeeyootUserId + "  ERROOORRRRRRRRRRRR ");
                    throw;
                }
            }
            //var qwe = new List<SelectListItem>();

            //var entriesProjection = orderEntities.Orders.Select(e =>
            //{
            //    return Shape.FaqEntry(
            //        PublicId: e.PublicId,
            //        Products: e.Products,
            //        Status: e.Status,
            //        EmailBuyer: e.EmailBuyer,
            //        Id: e.Id,
            //        Profit: e.Profit,
            //        UserNameSeller: e.UserNameSeller,
            //        Payout: e.Payout,
            //        CampaignId: e.CampaignId,
            //        CampaignName: e.CampaignName,
            //        CampaignAlias: e.CampaignAlias,
            //        SellerId: e.SellerId,
            //        CreateDate: e.CreateDate.ToString("dd/MM/yyyy")
            //        );
            //});
            //var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);

            //var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            var orderStatuses = _orderStatusRepository.Table
                .Select(s => new OrderStatusItemViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList();

            var currencies = _currencyRepository.Table
                .Select(c => new CurrencyItemViewModel
                {
                    Id = c.Id,
                    Name = c.ShortName
                });

            return View("Index", new AdminOrderViewModel
            {
                DynamicOrders = orderEntities.Orders.ToArray(),
                OrderStatuses = orderStatuses,
                SelectedCurrencyFilterId = filterCurrencyId,
                Currencies = currencies
            });
        }

        public JsonResult GetOrderInfirmation(string publicId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));

            var products = order.Products.Select(o => new
            {
                Name = o.CampaignProductRecord.ProductRecord.Name,
                Count = o.Count,
                Currency = o.CampaignProductRecord.CurrencyRecord.Code,
                Price =
                    o.CampaignProductRecord.Price +
                    Pricing(o.CampaignProductRecord.ProductRecord.SizesAvailable, o.ProductSizeRecord.Id),
                Size = o.ProductSizeRecord.SizeCodeRecord.Name,
                Color =
                    o.ProductColorRecord == null
                        ? o.CampaignProductRecord.ProductColorRecord.Value
                        : o.ProductColorRecord.Value
            });

            var totalPrice = order.TotalPriceWithPromo > 0.0 ? order.TotalPriceWithPromo : order.TotalPrice;
            totalPrice += order.Delivery;

            var result = new {products, totalPrice};
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private float Pricing(IList<LinkProductSizeRecord> SizesAvailable, int productSizeRecord)
        {
            float sizeC = 0;
            foreach (var size in SizesAvailable)
            {
                if (size.ProductSizeRecord.Id == productSizeRecord)
                    sizeC = size.SizeCost;
            }
            return sizeC;
        }

        public JsonResult GetBuyerInfirmation(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            return Json(new
            {
                firstName = order.FirstName,
                lastName = order.LastName,
                streetAdress = order.StreetAddress,
                city = order.City,
                country = order.Country,
                phoneNumber = order.PhoneNumber,
                state = order.State,
                postalCode = order.PostalCode
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditStatusPayout(string publicId, double profit, int sellerId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));
            order.Paid = DateTime.Now.ToUniversalTime();
            var campaignId = order.Products.First().CampaignProductRecord.CampaignRecord_Id;
            var campaign = _campaignService.GetCampaignById(campaignId);
            order.ProfitPaid = true;
            _orderService.UpdateOrder(order);
            _payoutService.AddPayout(new PayoutRecord
            {
                Date = DateTime.Now.ToUniversalTime(),
                Currency_Id = order.CurrencyRecord.Id,
                Amount = profit,
                IsPlus = true,
                Status = "Completed",
                UserId = sellerId,
                Event = publicId.Trim(' '),
                IsOrder = true
            });
            return RedirectToAction("Index");
        }

        public ActionResult DeletePayout(string publicId)
        {
            if (_payoutService.DeletePayoutByOrderPublicId(publicId.Trim(' ')))
            {
                var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));
                order.Paid = null;
                order.ProfitPaid = false;
                _orderService.UpdateOrder(order);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ApplyStatus(int orderId, string orderStatus)
        {
            var order = _orderService.GetOrderById(orderId);
            var campId = order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id;

            if ((order.OrderStatusRecord.Name == OrderStatus.Cancelled.ToString() ||
                 order.OrderStatusRecord.Name == OrderStatus.Unapproved.ToString()) &&
                (orderStatus != OrderStatus.Cancelled.ToString() || orderStatus != OrderStatus.Unapproved.ToString()) &&
                order.OrderStatusRecord.Name != orderStatus)
            {
                var sum = order.Products.Select(o => o.Count).Sum();
                var campaign =
                    _campaignService.GetCampaignById(
                        order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id);
                campaign.ProductCountSold = campaign.ProductCountSold + sum;
            }

            if ((order.OrderStatusRecord.Name != OrderStatus.Cancelled.ToString() ||
                 order.OrderStatusRecord.Name != OrderStatus.Unapproved.ToString()) &&
                order.OrderStatusRecord.Name != orderStatus &&
                (orderStatus == OrderStatus.Cancelled.ToString() || orderStatus == OrderStatus.Unapproved.ToString()))
            {
                var sum = order.Products.Select(o => o.Count).Sum();
                var campaign =
                    _campaignService.GetCampaignById(
                        order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id);
                campaign.ProductCountSold = campaign.ProductCountSold - sum;
            }
            int i = 0;
            OrderStatus newStatus = (OrderStatus) Enum.Parse(typeof (OrderStatus), orderStatus);
            if (orderStatus == "Shipped")
            {
                order.WhenSentOut = DateTime.UtcNow;
            }
            _orderService.UpdateOrder(order, newStatus);
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var pathToMedia = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
            //ToDo: delete order if new status is "Cancelled"

            var ordersForCampaign = _orderService.GetProductsOrderedOfCampaign(campId);
            foreach (var item in ordersForCampaign)
            {
                if (item.OrderRecord.OrderStatusRecord.Name != "Delivered")
                {
                    i++;
                }
            }
            if (i == 0)
            {
                _teeyootMessagingService.SendAllOrderDeliveredMessageToSeller(
                    (order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id));
            }
            _teeyootMessagingService.SendOrderStatusMessage(pathToTemplates, pathToMedia, orderId, orderStatus);
            _notifierService.Information(T("Successfully updated order status "));
            return RedirectToAction("Index");
        }
    }
}