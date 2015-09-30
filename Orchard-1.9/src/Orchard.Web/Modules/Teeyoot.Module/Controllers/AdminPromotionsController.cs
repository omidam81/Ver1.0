using Orchard.UI.Admin;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;


namespace Teeyoot.Module.Controllers
{

    [Admin]
    public class AdminPromotionsController : Controller
    {

        private readonly IPromotionService _promotionService;


        public AdminPromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public ActionResult Index()
        {
            var promotionsList = _promotionService.GetAllPromotions().ToList();           
            return View("Index", promotionsList);
        }

        public void ChangeState(int id, bool switchState)
        {
            if (switchState)
            {
                _promotionService.ActivatePromotion(id);
            }
            else
            {
                _promotionService.DisablePromotion(id); 
            }


        }

        

    }
}