using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;


namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult MarketingCampaigns(PromotionViewModel viewModel)
        {
            int currentUser = Services.WorkContext.CurrentUser.Id;
            var model = new PromotionViewModel() { };
           var currencies = _currencyRepository.Table.ToList();
            currencies.Add(new CurrencyRecord() {
                Code = "%"
            });
           model.AvailableCurrencies = from c in currencies
                                       select new SelectListItem
                                       {                                   
                                           Text = c.Code,
                                           Value = c.Code
                                       };
           
           model.Promotions =  _promotionService.GetAllPromotionsForUser(currentUser).ToList();
           model.Expiration = DateTime.Today;
           return View("Promotions",model);
           }

        public ActionResult AddPromotion(PromotionRecord model)
        {
            int currentUser = Services.WorkContext.CurrentUser.Id;
            _promotionService.AddPromotion(model.PromoId, model.DiscountType, model.AmountSize, model.AmountType, model.Expiration, currentUser);
            var viewModel = new PromotionViewModel() { };
            return RedirectToAction("MarketingCampaigns");
        }

        public ActionResult DeletePromotion(int id)
        {
            _promotionService.DeletePromotion(id);
            return RedirectToAction("MarketingCampaigns");
        }

        [HttpPost]
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

        public ActionResult DisablePromotion(int id)
        {
            _promotionService.DisablePromotion(id);
            return RedirectToAction("MarketingCampaigns");
        }

    }
}