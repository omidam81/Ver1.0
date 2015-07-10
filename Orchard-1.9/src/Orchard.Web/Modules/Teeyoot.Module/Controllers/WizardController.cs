using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Controllers
{
    [Themed]
    public class WizardController : Controller
    {
        // GET: Wizard
        public ActionResult Index()
        {
            return View();
        }
    }
}