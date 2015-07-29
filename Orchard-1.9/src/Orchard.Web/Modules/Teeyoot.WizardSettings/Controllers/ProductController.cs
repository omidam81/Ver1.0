using System;
using System.Web.Mvc;
using Orchard.UI.Admin;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class ProductController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddProduct()
        {
            throw new NotImplementedException();
        }
    }
}