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
using Teeyoot.Module.Services;
using Teeyoot.WizardSettings.ViewModels;
using System.Collections.Generic;

namespace Teeyoot.WizardSettings.Controllers
{
    [Admin]
    public class AdminWizardController : Controller
    {
        private readonly IFontService _fontService;
        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        public AdminWizardController(IFontService fontService, IOrchardServices services, IWorkContextAccessor workContextAccessor)
        {
            _fontService = fontService;
            Services = services;

            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        public ActionResult FontList()
        {
            var fonts = _fontService.GetAllfonts().Where(f => f.FontCulture == cultureUsed).ToList();
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
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName).Replace("-webfont", "");
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/fonts/"), file.FileName);
                    file.SaveAs(path);
                    model.WoffFile = file.FileName;
                    model.FileName = fileName;
                    Services.Notifier.Information(T("WOFF file has been added!"));
                    return RedirectToAction("AddFont", model);
                }
            }
            Services.Notifier.Error(T("Wrong file extention!"));
            return RedirectToAction("AddFont", model);
        }

        [HttpPost]
        public RedirectToRouteResult UploadThumbnail(HttpPostedFileBase file, FontViewModel model)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".png"};
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
            Services.Notifier.Error(T("Wrong file extention!"));
            return RedirectToAction("AddFont", model);
        }

        [ValidateAntiForgeryToken]
        public ActionResult DeleteFont(int id, string returnUrl)
        {
            var record = _fontService.GetFont(id);
            var fontsPath = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/fonts/"));
            string searchPattern = record.FileName + ".*";
            Array.ForEach(Directory.GetFiles(fontsPath, searchPattern), delegate(string path) { System.IO.File.Delete(path); });
            _fontService.DeleteFont(id);
            Services.Notifier.Information(T("The font has been deleted."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("FontList"));
        }

        
        public ActionResult UpdateFont(int id, string family, string tags)
        {

            FontRecord newFont = new FontRecord() { };
            newFont.Id = id;
            newFont.Family = family;
            newFont.Priority = 0;
            int i = 0;
            if (tags != null)
            {
                string[] stringSeparators = new string[] { "," };
                string[] separatedTags;
                string resultTags = "[";
                separatedTags = tags.Split(stringSeparators, StringSplitOptions.None);
                foreach (var item in separatedTags)
                {
                    if (i != 0)
                    {
                        resultTags = resultTags + ",";
                    }
                    resultTags = resultTags + "\"" + item + "\"";
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
            newFont.Priority = 0;
            newFont.FontCulture = cultureUsed;
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