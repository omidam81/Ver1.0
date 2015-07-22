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
using Orchard.UI.Navigation;
using Orchard.Settings;
using Orchard.DisplayManagement;

namespace Teeyoot.Search.Controllers
{
    [Admin]
    public class AdminSearchController : Controller
    {
        private readonly ICampaignService _campService;
        private readonly ICampaignCategoriesService _campCategService;
        private readonly ISiteService _siteService;

        public Localizer T { get; set; }
        private dynamic Shape { get; set; }

        private const int takeCategories = 20;
        public List<ActionsViewModel> Actions = new List<ActionsViewModel>() { 
            new ActionsViewModel() { Id = 1, Name = "Delete" },
            new ActionsViewModel() { Id = 2, Name = "Visible" }, 
            new ActionsViewModel() { Id = 3, Name = "Unvisible" } 
        };

        public AdminSearchController(IOrchardServices services, ICampaignService campService, ICampaignCategoriesService campCategService, ISiteService siteService, IShapeFactory shapeFactory)
        {
            Services = services;
            _campService = campService;
            _campCategService = campCategService;
            _siteService = siteService;
            Shape = shapeFactory;
        }

        private IOrchardServices Services { get; set; }


        public ActionResult Index(PagerParameters pagerParameters, AdminSearchViewModel adminViewModel)
        {
            var categoriesList = _campCategService.GetAllCategories().OrderBy(c => c.Name).ToList();

            if (!string.IsNullOrEmpty(adminViewModel.SearchString))
            {
                categoriesList = categoriesList.Where(c => c.Name.ToLower().Contains(adminViewModel.SearchString)).OrderBy(c => c.Name).ToList();
            }

            var entriesProjection = categoriesList.Select(e =>
            {
                return Shape.FaqEntry(
                    Id: e.Id,
                    Name: e.Name,
                    IsVisible: e.IsVisible
                    );
            });

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            return View("Index", new AdminSearchViewModel { CampaignCategoriesList = entries.ToArray(), Action = Actions, Pager = pagerShape });
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl)
        {

            //Services.Notifier.Information(T("The Search Category has been deleted."));
            //return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            return null;
        }

        public ActionResult EditCategory(PagerParameters pagerParameters, int id, string searchString)
        {
            var camps = _campCategService.GetCampaignsByIdCategory(id).ToList();
            CampaignCategoriesRecord category = _campCategService.GetCategoryById(id);

            if (!string.IsNullOrEmpty(searchString))
            {
                camps = camps.Where(c => c.Title.Contains(searchString)).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).ToList();
            }

            var entriesProjection = camps.Select(e =>
            {
                return Shape.FaqEntry(
                    Id: e.Id,
                    Title: e.Title,
                    ProductCountSold: e.ProductCountSold,
                    ProductCountGoal: e.ProductCountGoal
                    );
            });
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            return View("EditCategory", new AdminSearchViewModel { Camapigns = entries.ToArray(), CategoryId = id, CategoryName = category.Name, Pager = pagerShape });
        }

        public ActionResult ChangesVisibleCategory(int id, bool visible)
        {
            if (!_campCategService.CnehgeVisible(id, visible))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Error when chenged visible category. Try again!"));
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddCampaignForCategory(PagerParameters pagerParameters, int id, string searchString)
        {
            CampaignCategoriesRecord category = _campCategService.GetCategoryById(id);
            var camps = _campCategService.GetCampaignsByNotThisIdCategory(id);

            if (!string.IsNullOrEmpty(searchString))
            {
                camps = camps.Where(c => c.Title.Contains(searchString)).OrderByDescending(c => c.ProductCountSold).OrderBy(c => c.Title).ToList();
            }

            var entriesProjection = camps.Select(e =>
            {
                return Shape.FaqEntry(
                    Id: e.Id,
                    Title: e.Title,
                    ProductCountSold: e.ProductCountSold,
                    ProductCountGoal: e.ProductCountGoal
                    );
            });
            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters.Page, pagerParameters.PageSize);
            var entries = entriesProjection.Skip(pager.GetStartIndex()).Take(pager.PageSize);
            var pagerShape = Shape.Pager(pager).TotalItemCount(entriesProjection.Count());

            return View("AddCampaign", new AdminSearchViewModel { CategoryName = category.Name, Camapigns = entries.ToArray(), Pager = pagerShape, CategoryId = id });
        }

        public ActionResult AddNewCategory(AdminSearchViewModel adminViewModel)
        {
            if (!string.IsNullOrEmpty(adminViewModel.NewCategory))
            {
                if (_campCategService.AddCategory(adminViewModel.NewCategory) > 0)
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The New Category has been created."));
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Warning, T("This Category already exists!"));
                }
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Please, enter name of the Category"));
            }

            //CampaignCategoriesPart campaignCategoryPart = Services.ContentManager.New<CampaignCategoriesPart>("CampaignCategories");
            //var model = Services.ContentManager.BuildEditor(campaignCategoryPart);


            return this.RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            if (!_campCategService.DeleteCategory(id))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Category was not removed. Try again!"));
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The Category has been deleted."));
            }
            return this.RedirectToAction("Index");
        }

        public ActionResult ChangeNameCategory(AdminSearchViewModel adminViewModel)
        {
            if (!string.IsNullOrEmpty(adminViewModel.NewCategory))
            {
                if (_campCategService.ChnageNameCategory(adminViewModel.CategoryId, adminViewModel.NewCategory))
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The name was chenged successfully!"));
                }
                else
                {
                    Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Can not change name for category. Try again late!"));
                }
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Warning, T("You did not enter a name for the update"));
            }

            return this.RedirectToAction("EditCategory", new { id = adminViewModel.CategoryId });
        }

        public ActionResult SearchCampaigns(PagerParameters pagerParameters, AdminSearchViewModel adminViewModel)
        {
            return this.RedirectToAction("EditCategory", new { id = adminViewModel.CategoryId, searchString = adminViewModel.SearchString });
        }

        public ActionResult DeleteCampaignsFromCategory(int id, int categId)
        {
            if (_campService.DeleteCampaignFromCategoryById(id, categId))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The Campaign has been deleted from this category."));
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Can not delete campaign from category. Try again late!"));
            }
            return this.RedirectToAction("EditCategory", new { id = id });
        }

        public ActionResult SearchCampaignForCategory(PagerParameters pagerParameters, AdminSearchViewModel adminViewModel)
        {
            return this.RedirectToAction("AddCampaignForCategory", new { id = adminViewModel.CategoryId, searchString = adminViewModel.SearchString });
        }

        public ActionResult AddCampaign(int idCamp, int idCateg, string stringSearch)
        {
            if (_campCategService.AddCampaignToCategory(idCamp, idCateg))
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Information, T("The campaign has successfully been added to the category."));
            }
            else
            {
                Services.Notifier.Add(Orchard.UI.Notify.NotifyType.Error, T("Can not added campaign to the category. Try again late!"));
            }

            return this.RedirectToAction("AddCampaignForCategory", new { id = idCateg, searchString = stringSearch });
        }
    }
}