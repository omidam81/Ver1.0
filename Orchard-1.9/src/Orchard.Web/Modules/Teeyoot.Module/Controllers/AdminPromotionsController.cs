using Orchard.UI.Admin;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Teeyoot.Module.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Orchard.Users.Models;
using Orchard.ContentManagement;
using System.Collections.Generic;
using Orchard.Data;


namespace Teeyoot.Module.Controllers
{

    [Admin]
    public class AdminPromotionsController : Controller
    {

        private readonly IPromotionService _promotionService;
        private readonly IContentManager _contentManager;
        private readonly IRepository<PromotionRecord> _promotionRepository;


        public AdminPromotionsController(IPromotionService promotionService, IContentManager contentManager, IRepository<PromotionRecord> promotionRepository)
        {
            _promotionService = promotionService;
            _contentManager = contentManager;
            _promotionRepository = promotionRepository;
        }

        public ActionResult Index()
        {
            var promotionsList = _promotionService.GetAllPromotions().ToList();
            List<AdminPromotionViewModel> modelList = new List<AdminPromotionViewModel>();
            foreach (var promo in promotionsList)
            {
                var userEmail = "";
                var user = _contentManager.Query<UserPart, UserPartRecord>().List().FirstOrDefault(usr => usr.Id == promo.UserId);
                if (user != null)
                {
                    userEmail = user.Email;
                }
                var model = new AdminPromotionViewModel()
                {
                    Promotion = promo,
                    CampaignerEmail = userEmail
                };
                modelList.Add(model);
            }
            return View("Index", modelList);
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


        public HttpStatusCodeResult ChangePromotion(int id, double amount)
        {
            var promotion = _promotionRepository.Get(id);
            if (promotion != null)
            {
                promotion.AmountSize = amount;
                _promotionRepository.Flush();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        

    }
}