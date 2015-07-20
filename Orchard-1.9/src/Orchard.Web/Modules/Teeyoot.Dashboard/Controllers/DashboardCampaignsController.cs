using System.Web.Mvc;
using System.Linq;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Campaigns()
        {
            var model = new CampaignsViewModel();
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var campaigns = _campaignService.GetCampaignsOfUser(teeyootUser != null ? teeyootUser.Id : 0);



            return View(model);
        }
    }
}