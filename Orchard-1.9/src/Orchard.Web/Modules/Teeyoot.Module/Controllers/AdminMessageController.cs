using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.Module.Controllers
{
    public class AdminMessageController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSetting()
        {
            return View();
        }
    }
}