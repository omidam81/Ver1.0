using Orchard;
using Orchard.Security;
using System.Web.Mvc;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        private readonly IMembershipService _membershipService;
       // private readonly IWorkContextAccessor _wca;


        public DashboardController(IMembershipService membershipService, IWorkContextAccessor wca)
        {
            _membershipService = membershipService;
        }
        
        
        public ActionResult Settings()
        {
            var user = _wca.GetContext().HttpContext.User; 
            
            return View();
        }
    }
}