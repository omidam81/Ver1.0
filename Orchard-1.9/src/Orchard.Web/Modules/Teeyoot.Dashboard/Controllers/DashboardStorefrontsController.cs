using System.Web.Mvc;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Storefronts()
        {
            return View();
        }
    }
}