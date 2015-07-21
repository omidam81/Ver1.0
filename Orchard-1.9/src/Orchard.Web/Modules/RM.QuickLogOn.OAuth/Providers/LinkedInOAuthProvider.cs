using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.Providers;
using System.Web.Mvc;
using Orchard.Localization;

namespace RM.QuickLogOn.OAuth.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.LinkedIn")]
    public class LinkedInOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id={0}&scope=r_basicprofile%20r_emailaddress&state={2}&redirect_uri={1}";

        private Localizer T = NullLocalizer.Instance;

        public string Name
        {
            get { return T("LinkedIn").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your LinkedIn account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<LinkedInSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "LinkedInOAuth", new { Area = "RM.QuickLogOn.OAuth", ReturnUrl = returnUrl })).ToString();//
            var state = Guid.NewGuid().ToString().Trim('{', '}');
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl), state);
        }
    }
}
