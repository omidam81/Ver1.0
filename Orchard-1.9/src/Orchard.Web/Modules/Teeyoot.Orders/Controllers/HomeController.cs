using Orchard.UI.Admin;
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
        // GET: Home

        public HomeController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        public ActionResult Index()
        {
            var orders = _orderService.GetAllOrders();
            var orderEntities = new AdminOrderViewModel();
            foreach (var item in orders)
            {
                
            }
            return View();
        }
    }
}