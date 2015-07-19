using System.Web.Mvc;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Campaigns()
        {
            var campaigns = _campaignService.GetAllCampaigns();
            return View();
        }
    }
}