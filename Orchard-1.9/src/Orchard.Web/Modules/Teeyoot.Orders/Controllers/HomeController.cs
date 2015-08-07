﻿using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
        private readonly IOrderService _orderService;
        private readonly ICampaignService _campaignService;
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IPayoutService _payoutService;
        private readonly INotifier _notifierService;

        private dynamic Shape { get; set; }
        // GET: Home

        public HomeController(IOrderService orderService,
                              ICampaignService campaignService,
                              IShapeFactory shapeFactory,
                              IContentManager contentManager,
                              ISiteService siteService,
                              IPayoutService payoutService,
                              INotifier notifierService)
        {
            _orderService = orderService;
            _campaignService = campaignService;
            _contentManager = contentManager;
            _siteService = siteService;
            _payoutService = payoutService;
            _notifierService = notifierService;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
        }


        public Localizer T { get; set; }
        public ActionResult Index(PagerParameters pagerParameters, string searchString)
        {
            var orders = _orderService.GetAllOrders().Where(o => o.IsActive).ToList();
            var orderEntities = new AdminOrderViewModel();
            foreach (var item in orders)
            {
                var campaignId = item.Products.First().CampaignProductRecord.CampaignRecord_Id;
                var campaign = _campaignService.GetCampaignById(campaignId);
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);
                double orderProfit = 0;
                //Profit
                foreach (var product in item.Products)
                {
                    var prof = product.CampaignProductRecord.Price - product.CampaignProductRecord.BaseCost;
                    foreach (var size in product.CampaignProductRecord.ProductRecord.SizesAvailable)
                    {
                        if (size.Id == product.ProductSizeRecord.Id)
                            prof = prof - size.SizeCost;
                    }
                    orderProfit = orderProfit + prof;
                }

                if (string.IsNullOrWhiteSpace(searchString) || campaign.Title.ToLower().Contains(searchString.ToLower()))
                {
                    orderEntities.Orders.Add(new AdminOrder  {
                    PublicId = item.OrderPublicId,
                    Products = item.Products,
                    Status = item.OrderStatusRecord.Name,
                    EmailBuyer = item.Email,
                    CampaignName = campaign.Title,
                    Id = item.Id,
                    Profit = orderProfit,
                    SellerId = seller.Id,
                    //FirstName = item.FirstName,
                    //LastName = item.LastName,
                    //StreetAdress = item.StreetAddress,
                    //City = item.City,
                    //Country = item.Country,
                    //PhoneNumber = item.PhoneNumber,
                    Payout = item.ProfitPaid,
                    UserNameSeller = seller.UserName
                   });
            }
            }
            //var qwe = new List<SelectListItem>();
            
            var entriesProjection = orderEntities.Orders.Select(e =>
            {
                return Shape.FaqEntry(
                    PublicId: e.PublicId,
                    Products: e.Products,
                    Status: e.Status,
                    EmailBuyer: e.EmailBuyer,
                    Id: e.Id,
                    Profit: e.Profit,
                    UserNameSeller: e.UserNameSeller,
                    Payout: e.Payout,
                    CampaignName: e.CampaignName,
                    SellerId: e.SellerId
                    );
            });
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            return View("Index", new AdminOrderViewModel { DynamicOrders = entries.ToArray(), Pager = pagerShape });
        }

        public JsonResult GetOrderInfirmation(string publicId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));
            var orders = order.Products.Select(o => new { Name = o.CampaignProductRecord.ProductRecord.Name, Count = o.Count, Price = o.CampaignProductRecord.Price, Size = o.ProductSizeRecord.SizeCodeRecord.Name });
            return Json(orders, JsonRequestBehavior.AllowGet);
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
                phoneNumber = order.PhoneNumber
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditStatusPayout(string publicId, double profit, int sellerId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));
            var campaignId = order.Products.First().CampaignProductRecord.CampaignRecord_Id;
            var campaign = _campaignService.GetCampaignById(campaignId);
            order.ProfitPaid = true;
            _orderService.UpdateOrder(order);
            _payoutService.AddPayout(new PayoutRecord { Date = DateTime.Now, Amount = profit, IsPlus = true, Status = "Completed", UserId = sellerId, Event = campaign.Alias });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ApplyStatus(int orderId, string orderStatus)
        {
            var order = _orderService.GetOrderById(orderId);
            OrderStatus newStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
            _orderService.UpdateOrder(order, newStatus);
            //_notifierService.Information();


            _notifierService.Information(T("Successfully updated order status "));
            return RedirectToAction("Index");
        }


    }
}