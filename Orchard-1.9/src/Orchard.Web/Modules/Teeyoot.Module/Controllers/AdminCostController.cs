using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Controllers
{
    public class AdminCostController : Controller
    {
        [Admin]
        // GET: AdminCost
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }
    }
}