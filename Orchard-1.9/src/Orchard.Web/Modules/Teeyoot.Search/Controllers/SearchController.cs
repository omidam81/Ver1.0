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
        private readonly ICampaignProductService _campProductService;

        private List<CampaignRecord> campListAfterSearch;
        private List<CampaignProductRecord> campProductList;

        public SearchController(ICampaignService campService, ICampaignCategoriesService campCategService, ICampaignProductService campProductService)
        {
            _campService = campService;
            _campCategService = campCategService;
            _campProductService = campProductService;
        }

        [HttpGet]
        public ActionResult Search(string filter, int skip = 0, int take = 16)
        {
            filter = filter.Trim();

            if (!string.IsNullOrEmpty(filter))
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(filter, skip, take).ToList();
            }
            else
            {
                campListAfterSearch = _campService.GetAllCampaigns().OrderByDescending(c => c.ProductCountSold).Skip(skip).Take(take).ToList();
            }

            bool notResult = CheckResult();

            float[] price = PriceForCampaign(notResult);

            return View(new SearchViewModel { NotResult = notResult, Filter = filter, CampList = campListAfterSearch, Price = price });
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

            float[] price = PriceForCampaign(notResult);

            return View(new SearchViewModel { NotResult = notResult, Filter = categoriesName, CampList = campListAfterSearch, NewRow = 0, NotFoundCategories = notFoundCateg, CampCategList = campCategList, Price = price});
        }

        private float[] PriceForCampaign(bool notRes)
        {
            campProductList = notRes == false ? _campProductService.GetCampaignProductsByCampaign(campListAfterSearch) : null;
            float[] price = new float[campListAfterSearch.Count];
            float? addPrice;
            if (campProductList != null)
            {
                for (int i = 0; i < campListAfterSearch.Count; i++)
                {
                    addPrice = (float)campProductList.Where(c => c.CampaignRecord_Id == campListAfterSearch[i].Id).Select(c => c.Price).FirstOrDefault();
                    price[i] = addPrice != null ? (float)addPrice : 0;
                }
            }

            return price;
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