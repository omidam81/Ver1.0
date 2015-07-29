using Orchard.ContentManagement;
using Orchard.Users.Models;
using Orchard.Users.ViewModels;
using System.Web.Mvc;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
      
        public ActionResult Settings()
        {
            string currentUser = Services.WorkContext.CurrentUser.Email;
            var user = _membershipService.GetUser(currentUser);
            UserSettingsViewModel model = new UserSettingsViewModel() { };
            return View();
        }
    }
}