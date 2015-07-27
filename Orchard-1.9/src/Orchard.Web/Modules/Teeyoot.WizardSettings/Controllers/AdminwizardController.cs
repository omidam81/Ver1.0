using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.Services;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class AdminWizardController : Controller
    {
        private readonly IFontService _fontService;
        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public AdminWizardController(IFontService fontService, IOrchardServices services)
        {
            _fontService = fontService;
            Services = services;
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


        public ActionResult AddFont(FontRecord record)
        {
            if (record != null)
            {
                return View(record);
            }
            return View();               
        }


        public ActionResult EditFont(int id)
        {
            FontRecord record = _fontService.GetFont(id);
            return View(record);
        }
        
        [HttpPost]
        public RedirectToRouteResult UploadWoffFile(HttpPostedFileBase file)
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
                    FontRecord rec = new FontRecord() { };
                    rec.FileName = file.FileName;
                    return RedirectToAction("AddFont", rec);
                }
            }
            return null;
        }

        [HttpPost]
        public RedirectToRouteResult UploadTtfFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".ttf" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    string fileName = Guid.NewGuid() + fileExt;
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/uploads/"), fileName);
                    file.SaveAs(path);
                    FontRecord rec = new FontRecord() { };
                    rec.FileName = file.FileName;
                    return RedirectToAction("AddFont", rec);
                }
            }
            return null;
        }

        [ValidateAntiForgeryToken] 
        public ActionResult DeleteFont(int id, string returnUrl)
        {
            _fontService.DeleteFont(id);
            Services.Notifier.Information(T("The font has been deleted."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("FontList"));
        }

         [HttpPost]
        public ActionResult UpdateFont(FontRecord font)
        {
            _fontService.EditFont(font);
            Services.Notifier.Information(T("The font has been updated."));
            return RedirectToAction("FontList");
        }

         [HttpPost]
         [ValidateAntiForgeryToken] 
         public RedirectToRouteResult AddNewFont(FontRecord record)
         {
             _fontService.AddFont(record);
             Services.Notifier.Information(T("The font has been added"));
             return RedirectToAction("FontList");
         }
    }
}