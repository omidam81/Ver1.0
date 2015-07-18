using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Teeyoot.Module.Models;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LaunchCampaign(string ProductCountGoal, string profit, string itemOptions, string product, string campaignTitle, string description, string campaignLength, string url)
        {
            return Json(ProductCountGoal);
        }

        [HttpPost]
        public JsonResult UpoadArtFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileExt = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid() + fileExt;
                var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/uploads/"), fileName);
                file.SaveAs(path);
                return Json(new ArtUpload { Name = fileName });
            }
            return null;
        }
    }
}