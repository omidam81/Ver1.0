using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
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
        private readonly ITeeyootMessagingService _teeyootMessagingService;

        public ILogger Logger { get; set; }
        private dynamic Shape { get; set; }
        // GET: Home

        public HomeController(IOrderService orderService,
                              ICampaignService campaignService,
                              IShapeFactory shapeFactory,
                              IContentManager contentManager,
                              ISiteService siteService,
                              IPayoutService payoutService,
                              INotifier notifierService,
                              ITeeyootMessagingService teeyootMessagingService)
        {
            _orderService = orderService;
            _campaignService = campaignService;
            _contentManager = contentManager;
            _siteService = siteService;
            _payoutService = payoutService;
            _notifierService = notifierService;
            _teeyootMessagingService = teeyootMessagingService;
            Shape = shapeFactory;

            T = NullLocalizer.Instance;
        }


        public Localizer T { get; set; }
        public ActionResult Index(PagerParameters pagerParameters)
        {
            var orders = _orderService.GetAllOrders().Where(o => o.IsActive).ToList();
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

                var seller = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(user => user.Id == campaign.TeeyootUserId);

                double orderProfit = 0;


                if (item.Products != null)
                {
                    foreach (var product in item.Products)
                    {
                        var prof = product.CampaignProductRecord.Price - product.CampaignProductRecord.BaseCost;
                        foreach (var size in product.CampaignProductRecord.ProductRecord.SizesAvailable)
                        {
                            if (size.Id == product.ProductSizeRecord.Id)
                                prof = prof - size.SizeCost;
                        }
                        orderProfit = orderProfit + prof;
                        orderProfit = Math.Round(orderProfit, 2);
                    }
                }

                //if (string.IsNullOrWhiteSpace(searchString) || campaign.Title.ToLower().Contains(searchString.ToLower()))
                //{
                    orderEntities.Orders.Add(new AdminOrder  {
                    PublicId = item.OrderPublicId,
                    Products = item.Products,
                    Status = item.OrderStatusRecord.Name,
                    EmailBuyer = item.Email,
                    CampaignName = campaign.Title,
                    Id = item.Id,
                    Profit = orderProfit,
                    SellerId = seller != null ? seller.Id : 0,
                    //FirstName = item.FirstName,
                    //LastName = item.LastName,
                    //StreetAdress = item.StreetAddress,
                    //City = item.City,
                    //Country = item.Country,
                    //PhoneNumber = item.PhoneNumber,
                    Payout = item.ProfitPaid,
                    CreateDate = item.Created,
                    UserNameSeller = seller != null ? seller.UserName : ""
                   });
            //}
                                }
                catch (Exception ex)
                {

                    Logger.Error(ex,campaign.TeeyootUserId + "  ERROOORRRRRRRRRRRR " );
                    throw;
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
                    SellerId: e.SellerId,
                    CreateDate: e.CreateDate.ToString("dd/MM/yyyy")
                    );
            });
            //var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
           
            //var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            return View("Index", new AdminOrderViewModel { DynamicOrders = entriesProjection.ToArray() });
        }

        public JsonResult GetOrderInfirmation(string publicId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));

            var products = order.Products.Select(o => new { Name = o.CampaignProductRecord.ProductRecord.Name,
                Count = o.Count,
                Currency = o.CampaignProductRecord.CurrencyRecord.Code,
                Price = o.CampaignProductRecord.Price + Pricing(o.CampaignProductRecord.ProductRecord.SizesAvailable,o.ProductSizeRecord.Id),
                Size = o.ProductSizeRecord.SizeCodeRecord.Name,
                Color = o.ProductColorRecord == null ? o.CampaignProductRecord.ProductColorRecord.Value : o.ProductColorRecord.Value});
         
            var totalPrice = order.TotalPriceWithPromo > 0.0 ? order.TotalPriceWithPromo : order.TotalPrice;
            var result = new { products, totalPrice };
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
                phoneNumber = order.PhoneNumber
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EditStatusPayout(string publicId, double profit, int sellerId)
        {
            var order = _orderService.GetOrderByPublicId(publicId.Trim(' '));
            order.Paid = DateTime.Now.ToUniversalTime();
            var campaignId = order.Products.First().CampaignProductRecord.CampaignRecord_Id;
            var campaign = _campaignService.GetCampaignById(campaignId) ;
            order.ProfitPaid = true;
            _orderService.UpdateOrder(order);
            _payoutService.AddPayout(new PayoutRecord { Date = DateTime.Now.ToUniversalTime(), Currency_Id = order.CurrencyRecord.Id, Amount = profit, IsPlus = true, Status = "Completed", UserId = sellerId, Event = campaign.Alias });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ApplyStatus(int orderId, string orderStatus)
        {
            var order = _orderService.GetOrderById(orderId);
            var campId = order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id;

            if (order.OrderStatusRecord.Name == OrderStatus.Cancelled.ToString() && orderStatus != OrderStatus.Cancelled.ToString())
            {
                var sum = order.Products.Select(o => o.Count).Sum();
                var campaign = _campaignService.GetCampaignById(order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id);
                campaign.ProductCountSold = campaign.ProductCountSold + sum;
            }

            if (order.OrderStatusRecord.Name != OrderStatus.Cancelled.ToString() && orderStatus == OrderStatus.Cancelled.ToString())
            {
                var sum = order.Products.Select(o => o.Count).Sum();
                var campaign = _campaignService.GetCampaignById(order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id);
                campaign.ProductCountSold = campaign.ProductCountSold - sum;
            }
            int i = 0;
            OrderStatus newStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus);
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
                _teeyootMessagingService.SendAllOrderDeliveredMessageToSeller((order.Products.FirstOrDefault().CampaignProductRecord.CampaignRecord_Id));
            }
            _teeyootMessagingService.SendOrderStatusMessage(pathToTemplates, pathToMedia, orderId, orderStatus);
            _notifierService.Information(T("Successfully updated order status "));
            return RedirectToAction("Index");
        }


    }
}