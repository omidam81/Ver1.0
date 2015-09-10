using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Search.Services;
using Teeyoot.Module.Services;
using Teeyoot.Module.Models;
using Teeyoot.Search.ViewModels;
using Orchard;

namespace Teebay.Search.Controllers
{
    [Themed]
    public class SearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;
        private readonly ICampaignProductService _campProductService;
        private readonly IWorkContextAccessor _workContextAccessor;

        private List<CampaignRecord> campListAfterSearch;
        private List<CampaignProductRecord> campProductList;
        private const int take = 16;

        public SearchController(ICampaignService campService, ICampaignCategoriesService campCategService, ICampaignProductService campProductService, IWorkContextAccessor workContextAccessor)
        {
            _campService = campService;
            _campCategService = campCategService;
            _campProductService = campProductService;
            _workContextAccessor = workContextAccessor;
        }

        [HttpGet]
        public ActionResult Search(string filter, int? page)
        {
            var searchViewModel = GetSearchViewModel(filter, page);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_CustomerRow", searchViewModel);
            }

            return View(searchViewModel);
        }

        [HttpGet]
        public ActionResult SearchCampaigns(string filter, int? page)
        {
            var searchViewModel = GetSearchViewModel(filter, page);

            return PartialView("SearchCampaigns", searchViewModel);
        }

        public ActionResult CategoriesSearch(string categoriesName)
        {
            categoriesName = categoriesName.Trim();

            List<CampaignCategoriesRecord> campCategList = _campCategService.GetAllCategories().ToList();
            CampaignCategoriesRecord findCampCateg = campCategList.Find(x => x.Name.ToLower() == categoriesName.ToLower());
            bool notFoundCateg = false;

            string culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureSearch = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            if (findCampCateg != null)
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(categoriesName.ToLower(), 0, 16, true).Where(c => c.CampaignCulture == cultureSearch).ToList();
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

        private SearchViewModel GetSearchViewModel(string filter, int? page)
        {
            page = page ?? 0;
            int skip = (int)page * take;

            filter = filter.Trim();

            string culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureSearch = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            if (!string.IsNullOrEmpty(filter))
            {
                campListAfterSearch = _campService.GetCampaignsForTheFilter(filter, skip, take).Where(c => c.CampaignCulture == cultureSearch).ToList();
            }
            else
            {
                campListAfterSearch = _campService.GetAllCampaigns().Where(c => !c.IsPrivate && c.IsActive && c.IsApproved && c.CampaignCulture == cultureSearch).OrderByDescending(c => c.ProductCountSold).Skip(skip).Take(take).ToList();
            }

            bool notResult = CheckResult();

            float[] price = PriceForCampaign(notResult);

            var searchViewModel = new SearchViewModel
            {
                NotResult = notResult,
                Filter = filter,
                CampList = campListAfterSearch,
                Price = price
            };

            return searchViewModel;
        }
    }
}