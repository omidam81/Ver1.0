using Orchard;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Services;
using Teeyoot.Search.Services;
using Teeyoot.Module.Models;
using Teeyoot.Search.ViewModels;
using Orchard.Localization;

namespace Teeyoot.Search.Controllers
{
    [Admin]
    public class AdminSearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;

        private const int takeCategories = 20;

        public AdminSearchController(IOrchardServices services, ICampaignService campService, ICampaignCategoriesService campCategService)
        {
            Services = services;
            _campService = campService;
            _campCategService = campCategService;
        }

        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index()
        {
            List<CampaignCategoriesPartRecord> categoriesList = _campCategService.GetAllCategories().OrderBy(c => c.Name).Skip(0).Take(takeCategories).ToList();

            return View("Index", new AdminSearchViewModel { CampaignCategoriesList = categoriesList });
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl)
        {

            //Services.Notifier.Information(T("The Search Category has been deleted."));
            //return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            return null;
        }
    }
}