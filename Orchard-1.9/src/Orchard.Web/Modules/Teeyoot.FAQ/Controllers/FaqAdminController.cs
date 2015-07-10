using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.VirtualPath;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;
using Orchard.UI.Notify;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents.Settings;
using Orchard.Localization.Services;
using Teeyoot.FAQ.Models;
using Orchard;
using Teeyoot.FAQ.Services;

namespace Teeyoot.FAQ.Controllers
{
    [ValidateInput(false), Admin]
    public class FaqAdminController : Controller
    {
        private readonly ILanguageService _languageService;

        public FaqAdminController(IOrchardServices services, ILanguageService languageService)
        {
            _languageService = languageService;
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        private IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddFaqEntry(string returnUrl)
        {
            FaqEntryPart faqEntryPart = Services.ContentManager.New<FaqEntryPart>("FaqEntry");
            if (faqEntryPart == null)
                return HttpNotFound();

            try
            {
                faqEntryPart.Language = _languageService.GetLanguageByCode(CultureInfo.CurrentCulture.Name);
                var model = Services.ContentManager.BuildEditor(faqEntryPart);
                return View(model);
            }
            catch (Exception exception)
            {
                //Logger.Error(T("Creating widget failed: {0}", exception.Message).Text);
                //Services.Notifier.Error(T("Creating widget failed: {0}", exception.Message));
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            }
        }
	}
}