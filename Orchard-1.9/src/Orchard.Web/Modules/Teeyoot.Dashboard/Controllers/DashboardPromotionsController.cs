using System.Linq;
using System.Web.Mvc;
using Teeyoot.Module.Models;


namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Promotions()
        {
           var model =  _promotionService.GetAllPromotions().ToList();
           return View("Promotions",model);
        }

        public ActionResult AddPromotion(PromotionRecord model)
        {
            _promotionService.AddPromotion(model.PromoId, model.DiscountType, model.AmountSize, model.AmountType, model.Expiration);
            return RedirectToAction("Promotions");
        }

        public ActionResult DeletePromotion(int id)
        {
            _promotionService.DeletePromotion(id);
            return RedirectToAction("Promotions");
        }

        public ActionResult ActivatePromotion(int id)
        {
            _promotionService.ActivatePromotion(id);
            return RedirectToAction("Promotions");
        }

        public ActionResult DisablePromotion(int id)
        {
            _promotionService.DisablePromotion(id);
            return RedirectToAction("Promotions");
        }

    }
}