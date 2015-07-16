using Orchard.Themes;
using System.Web.Mvc;


namespace Teeyoot.Module.Dashboard.Controllers
{
    [Themed]
    public partial class DashboardController : Controller
    {
        public ActionResult Campaigns()
        {
            return View();
        }
    }
}