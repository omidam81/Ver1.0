﻿using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Services;
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

        private dynamic Shape { get; set; }
        // GET: Home

        public HomeController(IOrderService orderService,
                              ICampaignService campaignService,
                              IShapeFactory shapeFactory,
                              IContentManager contentManager,
                              ISiteService siteService)
        {
            _orderService = orderService;
            _campaignService = campaignService;
            _contentManager = contentManager;
            _siteService = siteService;
            Shape = shapeFactory; 
        }

        public ActionResult Index(PagerParameters pagerParameters, AdminOrderViewModel adminViewModel)
        {
            var orders = _orderService.GetAllOrders().ToList();
            var orderEntities = new AdminOrderViewModel();
            //_campaignService.GetAllCampaignProducts
            foreach (var item in orders)
            {

                var campaignId = item.Products.First().CampaignProductRecord.CampaignRecord_Id;
                var campaign = _campaignService.GetCampaignById(campaignId);
                
                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);

                orderEntities.Orders.Add(new AdminOrder  {
                    PublicId = item.OrderPublicId,
                    Products = item.Products,
                    Status = item.OrderStatusRecord.Name,
                    EmailBuyer = item.Email,
                    Id = item.Id,
                    //FirstName = item.FirstName,
                    //LastName = item.LastName,
                    //StreetAdress = item.StreetAddress,
                    //City = item.City,
                    //Country = item.Country,
                    //PhoneNumber = item.PhoneNumber,
                    UserNameSeller = seller.UserName
                   });
            }

            var entriesProjection = orderEntities.Orders.Select(e =>
            {
                return Shape.FaqEntry(
                    PublicId: e.PublicId,
                    Products: e.Products,
                    Status: e.Status,
                    EmailBuyer: e.EmailBuyer,
                    Id : e.Id,
                    UserNameSeller: e.UserNameSeller
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
            return Json(orders , JsonRequestBehavior.AllowGet);
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

        
    }
}