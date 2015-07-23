using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;
using Orchard.Environment.Extensions;
using RM.QuickLogOn.Providers;
using System.Web.Mvc;
using Orchard.Localization;

namespace RM.QuickLogOn.OAuth.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Twitter")]
    public class TwitterOAuthProvider : IQuickLogOnProvider
    {
        private Localizer T = NullLocalizer.Instance;

        public string Name
        {
            get { return T("Twitter").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your Twitter account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = urlHelper.Action("RequestToken", "TwitterOAuth", new { Area = "RM.QuickLogOn.OAuth", ReturnUrl = returnUrl });
            return redirectUrl;
        }
    }
}
