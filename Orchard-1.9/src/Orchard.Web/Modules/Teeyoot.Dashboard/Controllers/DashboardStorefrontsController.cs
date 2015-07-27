using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using System.Linq;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        public ActionResult Storefronts()
        {
            return View();
        }

        public ActionResult NewStorefront()
        {
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var campaigns = _campaignService.GetCampaignsOfUser(teeyootUser.Id).ToList();
            var model = new StoreViewModel();
            model.Campaigns = campaigns;
            return View(model);
        }

        public ActionResult CreateStorefront(StoreViewModel model)
        {
            //var newStore = _storeService.CreateStore(store);

            return View("NewStorefront");
        }
       

    }
}