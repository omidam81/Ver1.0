using Orchard;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Localization;
using Teeyoot.Module.Services;
using System.IO;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Mvc.Extensions;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Teeyoot.WizardSettings.ViewModels;

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


        public ActionResult AddFont(FontViewModel record)
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
            FontViewModel editRecord = new FontViewModel() { };
            editRecord.Id = record.Id;
            editRecord.Family = record.Family;
            editRecord.FileName = record.FileName;
            editRecord.Priority = record.Priority;
            string[] stringSeparators = new string[] { ","};
            string[] separatedTags;
            string Tags = record.Tags.Trim(new Char[] { '[', '*', ',', ']',' ', '.' });          
            Tags = Tags.Replace("\"","");
            string resultTags = "";
            separatedTags = Tags.Split(stringSeparators, StringSplitOptions.None);
            int i = 0;
            foreach (var item in separatedTags)
            {
                if (i != 0)
                {
                    resultTags = resultTags + "," + " ";
                }
                resultTags = resultTags + item;
                i++;
            }
            editRecord.Tags = resultTags;
            return View(editRecord);
        }

        [HttpPost]
        public RedirectToRouteResult UploadWoffFile(HttpPostedFileBase file, FontViewModel model)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".woff" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/fonts/"), file.FileName);
                    file.SaveAs(path);
                    model.WoffFile = file.FileName;
                    model.FileName = fileName;
                    Services.Notifier.Information(T("WOFF file has been added!"));
                    return RedirectToAction("AddFont", model);
                }
            }
            return null;
        }

        [HttpPost]
        public RedirectToRouteResult UploadThumbnail(HttpPostedFileBase file, FontViewModel model)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".png", ".jpeg" , ".jpg", ".gif"};
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/fonts/"), file.FileName);
                    file.SaveAs(path);
                    model.Thumbnail = file.FileName;
                    Services.Notifier.Information(T("Thumbnail has been added!"));
                    return RedirectToAction("AddFont", model);
                }
            }
            return null;
        }


        [HttpPost]
        public RedirectToRouteResult UploadTtfFile(HttpPostedFileBase file, FontViewModel model)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".ttf" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/fonts/"), file.FileName);
                    file.SaveAs(path);
                    model.TtfFile = file.FileName;
                    model.FileName = fileName;
                    Services.Notifier.Information(T("TTF file has been added!"));
                    return RedirectToAction("AddFont", model);
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
        public ActionResult UpdateFont(FontViewModel model)
        {

            FontRecord newFont = new FontRecord() { };
            newFont.Id = model.Id;
            newFont.Family = model.Family;
            newFont.FileName = model.FileName;
            newFont.Priority = model.Priority;
            int i = 0;
            if (model.Tags != null)
            {
                string[] stringSeparators = new string[] { "," };
                string[] separatedTags;
                string resultTags = "[";
                separatedTags = model.Tags.Split(stringSeparators, StringSplitOptions.None);
                foreach (var item in separatedTags)
                {
                    if (i != 0)
                    {
                        resultTags = resultTags + ",";
                    }
                    resultTags = resultTags + "\"" + item  + "\"";
                    i++;
                }
                resultTags += "]";
                newFont.Tags = resultTags;
            }
            _fontService.EditFont(newFont);
            Services.Notifier.Information(T("The font has been updated."));
            return RedirectToAction("FontList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToRouteResult AddNewFont(FontViewModel model)
        {
            string errorMsg = "";
            if (model.Family == null)
            {
                errorMsg += T("Font family is required.\n");
            }
            if (!((model.TtfFile == null) || (model.WoffFile == null)))
            {
                errorMsg += T("Font file is required.\n");
            }
            if (model.Thumbnail == null)
            {
                errorMsg += T("Thumbnail is required.\n");
            }
            if (errorMsg.Length > 0)
            {
                Services.Notifier.Error(T(errorMsg));
                return RedirectToAction("AddFont", model);
            }
            FontRecord newFont = new FontRecord() { };
            newFont.Family = model.Family;
            newFont.FileName = model.FileName;
            newFont.Priority = model.Priority;
            int i = 0;
            if (model.Tags != null)
            {
                string[] stringSeparators = new string[] { "," };
                string[] separatedTags;
                string resultTags = "[";
                separatedTags = model.Tags.Split(stringSeparators, StringSplitOptions.None);
                foreach (var item in separatedTags)
                {
                    if (i != 0)
                    {
                        resultTags = resultTags + ",";
                    }
                    resultTags = resultTags + "\"" + item  + "\"";
                    i++;
                }
                resultTags += "]";
                newFont.Tags = resultTags;
            }
            _fontService.AddFont(newFont);
            Services.Notifier.Information(T("The font has been added"));
            return RedirectToAction("FontList");
        }
    }
}