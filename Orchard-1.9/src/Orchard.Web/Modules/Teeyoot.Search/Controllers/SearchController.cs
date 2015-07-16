using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Search.Services;
using Teeyoot.Search.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Models;

namespace Teebay.Search.Controllers
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
        public ActionResult Index(string filter, int skip = 0, int take =  16)
        {
            filter = filter.Trim();

            List<CampaignRecord> campListAfterSearch;

            if (!string.IsNullOrEmpty(filter))
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(filter, skip, take).ToList();
            }
            else
            {
                campListAfterSearch = _campService.GetAllCampaigns().OrderByDescending(c => c.ProductCountSold).Skip(skip).Take(take).ToList();
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
            ViewBag.Count = 0;
            return View();
        }

        public ActionResult CategoriesSearch(string categoriesName)
        {
            return View();
        }
    }
}