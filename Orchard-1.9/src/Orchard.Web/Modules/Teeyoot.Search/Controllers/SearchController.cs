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
using Teeyoot.Search.ViewModels;

namespace Teebay.Search.Controllers
{
    [Themed]
    public class SearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;

        private List<CampaignRecord> campListAfterSearch;

        public SearchController(ICampaignService campService, ICampaignCategoriesService campCategService)
        {
            _campService = campService;
            _campCategService = campCategService;
        }

        [HttpGet]
        public ActionResult Search(string filter, int skip = 0, int take =  16)
        {
            filter = filter.Trim();

            //List<CampaignRecord> campListAfterSearch;

            if (!string.IsNullOrEmpty(filter))
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(filter, skip, take, false).ToList();
            }
            else
            {
                campListAfterSearch = _campService.GetAllCampaigns().OrderByDescending(c => c.ProductCountSold).Skip(skip).Take(take).ToList();
            }

            bool notResult = CheckResult();

            return View(new SearchViewModel { NotResult = notResult, Filter = filter, CampList = campListAfterSearch });
        }

        public ActionResult CategoriesSearch(string categoriesName)
        {
            categoriesName = categoriesName.Trim();

            List<CampaignCategoriesPartRecord> campCategList = _campCategService.GetAllCategories().ToList();
            CampaignCategoriesPartRecord findCampCateg = campCategList.Find(x => x.Name == categoriesName);
            bool notFoundCateg = false;
            if (findCampCateg != null)
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(categoriesName.ToLower(), 0, 16, true).ToList();
                campCategList.Remove(findCampCateg);
            }
            else
            {
                notFoundCateg = true;
            }

            bool notResult = CheckResult();

            return View(new SearchViewModel {NotResult = notResult, Filter = categoriesName, CampList = campListAfterSearch, NewRow = 0, NotFoundCategories = notFoundCateg, CampCategList = campCategList });
        }

        private bool CheckResult()
        {
            if ((campListAfterSearch != null) && (campListAfterSearch.Count == 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}