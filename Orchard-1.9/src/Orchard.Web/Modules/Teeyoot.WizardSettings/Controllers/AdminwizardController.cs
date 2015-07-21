using Orchard;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Localization;
using Teeyoot.WizardSettings.Services;
using System.IO;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class AdminWizardController : Controller
    {
        private readonly IFontService _fontService;

        public AdminWizardController(IFontService fontService)
        {
            _fontService = fontService;
        }

        public ActionResult FontList()
        {
            var fonts = _fontService.GetAllfonts().ToArray();
            return View(fonts);
        }

        public ActionResult ColorList()
        {
            return View();
        }

        
        [HttpPost]
        public string UploadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".woff" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid() + fileExt;
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/uploads/"), fileName);
                    file.SaveAs(path);
                    return fileName;
                }
            }
            return null;
        }
    }
}