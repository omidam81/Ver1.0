using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc.Extensions;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Themes;
using Orchard.UI.Notify;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.OAuth.ViewModels;

namespace RM.QuickLogOn.OAuth.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.QuickLogOn.OAuth.LinkedIn")]
    public class LinkedInOAuthController : Controller
    {
        private readonly IOrchardServices _services;
        private readonly ILinkedInOAuthService _oauthService;

        public Localizer T { get; set; }

        public LinkedInOAuthController(IOrchardServices services, ILinkedInOAuthService oauthService)
        {
            T = NullLocalizer.Instance;
            _services = services;
            _oauthService = oauthService;
        }

        public ActionResult Auth(LinkedInOAuthAuthViewModel model)
        {
            var response = _oauthService.Auth(_services.WorkContext, model.Code, model.Error, model.ReturnUrl);
            if (response.Error != null)
            {
                _services.Notifier.Add(NotifyType.Error, response.Error);
            }
            
            return this.RedirectLocal(response.ReturnUrl);
        }
    }
}
