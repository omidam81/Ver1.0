using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Services;
using Teeyoot.Module.Models;

namespace Teebay.Module.Controllers
{
    [Themed]
    public class SearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;

        public SearchController(ICampaignService campService, ICampaignCategoriesService campCategService)
        {
            _campService = campService;
            _campCategService = campCategService;
        }

        [HttpGet]
        public ActionResult Index(string filter)
        {
            filter = filter.Trim();

            List<CampaignRecord> campListAfterSearch;

            if (!string.IsNullOrEmpty(filter))
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(filter).ToList();
            }
            else
            {
                campListAfterSearch = _campService.GetAllCampaigns().OrderBy(c => c.ProductCountSold).ToList();
            }

            if (campListAfterSearch.Count == 0)
            {
                ViewBag.NotResult = true;
                ViewBag.Filter = filter;
            }
            else
            {
                ViewBag.NotResult = false;
                ViewBag.CampList = campListAfterSearch;
            }
            
            return View();
        }

        public ActionResult CategoriesSearch(string categoriesName)
        {
            return View();
        }
    }
}