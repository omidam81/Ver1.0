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
    [OrchardFeature("RM.QuickLogOn.OAuth")]
    public class GoogleOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={0}&redirect_uri={1}&scope={2}&state={3}";
        public const string Scope = "https://www.googleapis.com/auth/userinfo.email";

        private Localizer T = NullLocalizer.Instance;

        public string Name
        {
            get { return T("Google").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your Google account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<GoogleSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "GoogleOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl), urlHelper.Encode(Scope), urlHelper.Encode(returnUrl.ToString()));
        }
    }
}
