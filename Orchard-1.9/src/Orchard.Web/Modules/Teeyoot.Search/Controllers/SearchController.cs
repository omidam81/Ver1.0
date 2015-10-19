using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using Orchard.Themes;
using Teeyoot.Module.Messaging.CampaignService;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Search.Services;
using Teeyoot.Search.ViewModels;

namespace Teeyoot.Search.Controllers
{
    [Themed]
    public class SearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;
        private readonly ICampaignProductService _campProductService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private List<CampaignProductRecord> _campProductList;

        private const int Take = 16;

        private List<SearchCampaignItem> _searchCampaignItems;

        public SearchController(
            ICampaignService campService,
            ICampaignCategoriesService campCategService,
            ICampaignProductService campProductService,
            IWorkContextAccessor workContextAccessor)
        {
            _campService = campService;
            _campCategService = campCategService;
            _campProductService = campProductService;
            _workContextAccessor = workContextAccessor;
        }

        [HttpGet]
        public ActionResult Search(string filter, int? page)
        {
            page = page ?? 0;
            var skip = (int) page*Take;

            filter = filter.Trim();

            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            var cultureSearch = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            SearchCampaignsResponse searchCampaignsResponse;

            if (!string.IsNullOrEmpty(filter))
            {
                var searchCampaignsRequest = new SearchCampaignsRequest
                {
                    Filter = filter,
                    Culture = cultureSearch,
                    Skip = skip,
                    Take = Take
                };

                searchCampaignsResponse = _campService.SearchCampaignsForFilter(searchCampaignsRequest);
                _searchCampaignItems = searchCampaignsResponse.Campaigns;
            }
            else
            {
                var searchCampaignsRequest = new SearchCampaignsRequest
                {
                    Culture = cultureSearch,
                    Skip = skip,
                    Take = Take
                };

                searchCampaignsResponse = _campService.SearchCampaigns(searchCampaignsRequest);
                _searchCampaignItems = searchCampaignsResponse.Campaigns;
            }

            var notResult = CheckResult();

            var price = PriceForCampaign(notResult);

            if (Request.IsAjaxRequest())
            {
                var searchViewModel = new SearchViewModel
                {
                    NotResult = notResult,
                    Filter = filter,
                    Campaigns = _searchCampaignItems,
                    Price = price
                };

                return PartialView("_CustomerRow", searchViewModel);
            }
            else
            {
                var searchViewModel = new SearchViewModel
                {
                    NotResult = notResult,
                    Filter = filter,
                    Campaigns = _searchCampaignItems,
                    Price = price
                };

                return View(searchViewModel);
            }
        }

        public ActionResult CategoriesSearch(string categoriesName)
        {
            categoriesName = categoriesName.Trim();

            var campCategList = _campCategService.GetAllCategories().ToList();
            var findCampCateg =
                campCategList.Find(x => x.Name.ToLower() == categoriesName.ToLower());
            var notFoundCateg = false;

            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            var cultureSearch = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            if (findCampCateg != null)
            {
                var searchCampaignsRequest = new SearchCampaignsRequest
                {
                    Tag = categoriesName.ToLowerInvariant(),
                    Culture = cultureSearch,
                    Skip = 0,
                    Take = Take
                };

                var searchCampaignsResponse = _campService.SearchCampaignsForTag(searchCampaignsRequest);
                _searchCampaignItems = searchCampaignsResponse.Campaigns;

                campCategList.Remove(findCampCateg);
            }
            else
            {
                notFoundCateg = true;
            }

            var notResult = CheckResult();

            var price = PriceForCampaign(notResult);

            var searchViewModel = new SearchViewModel
            {
                NotResult = notResult,
                Filter = categoriesName,
                Campaigns = _searchCampaignItems,
                NewRow = 0,
                NotFoundCategories = notFoundCateg,
                CampCategList = campCategList,
                Price = price
            };

            return View(searchViewModel);
        }

        private float[] PriceForCampaign(bool notRes)
        {
            _campProductList = notRes == false
                ? _campProductService.GetCampaignProductsByCampaign(_searchCampaignItems.Select(c => c.Id))
                : null;
            var price = new float[_searchCampaignItems.Count];

            if (_campProductList == null)
                return price;

            for (var i = 0; i < _searchCampaignItems.Count; i++)
            {
                float? addPrice = (float) _campProductList.Where(c => c.CampaignRecord_Id == _searchCampaignItems[i].Id)
                    .Select(c => c.Price)
                    .FirstOrDefault();
                price[i] = (float) addPrice;
            }

            return price;
        }

        private bool CheckResult()
        {
            return _searchCampaignItems != null && !_searchCampaignItems.Any();
        }
    }
}