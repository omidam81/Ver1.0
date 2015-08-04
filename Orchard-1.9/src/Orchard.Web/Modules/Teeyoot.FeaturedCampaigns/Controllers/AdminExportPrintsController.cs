using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.FeaturedCampaigns.Controllers
{
    [Admin]
    public class AdminExportPrintsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}