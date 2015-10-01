using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminTranslationTextController : Controller
    {
        // GET: AdminTranslationText
        public ActionResult Index()
        {

            return View();
        }
    }
}