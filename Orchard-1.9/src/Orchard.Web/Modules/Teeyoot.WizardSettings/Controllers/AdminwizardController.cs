using Orchard;
using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Localization;
using Teeyoot.WizardSettings.Services;

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
    }
}