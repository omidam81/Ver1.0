using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminCostController : Controller
    {
        // GET: AdminCost
        public ActionResult Index()
        {
            return View("Index", new AdminCostViewModel {  });
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }

        public ActionResult Save(AdminCostViewModel costViewModel)
        {


            return this.RedirectToAction("Index");
        }
    }
}