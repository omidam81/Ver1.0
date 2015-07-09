using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teebay.Module.Controllers
{
    [Themed]
    public class SearchController : Controller
    {
        public ActionResult Index(string filtersearch)
        {
            
            return View(filtersearch);
        }
    }
}