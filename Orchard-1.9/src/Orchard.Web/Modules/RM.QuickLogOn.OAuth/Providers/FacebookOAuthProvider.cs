using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.Providers;

namespace RM.QuickLogOn.OAuth.Providers
{
    [OrchardFeature("RM.QuickLogOn.OAuth.Facebook")]
    public class FacebookOAuthProvider : IQuickLogOnProvider
    {
        public const string Url = "https://www.facebook.com/dialog/oauth?client_id={0}&response_type=code&scope=email&redirect_uri={1}&state={2}";

        public Localizer T { get; set; }

        public FacebookOAuthProvider()
        {
            T = NullLocalizer.Instance;
        }

        public string Name
        {
            get { return T("Facebook").ToString(); }
        }

        public string Description
        {
            get { return T("LogOn with Your Facebook account").ToString(); }
        }

        public string GetLogOnUrl(WorkContext context)
        {
            var urlHelper = new UrlHelper(context.HttpContext.Request.RequestContext);
            var part = context.CurrentSite.As<FacebookSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = context.HttpContext.Request.Url;
            var redirectUrl = new Uri(returnUrl, urlHelper.Action("Auth", "FacebookOAuth", new { Area = "RM.QuickLogOn.OAuth" })).ToString();//, returnUrl = returnUrl
            return string.Format(Url, clientId, urlHelper.Encode(redirectUrl), urlHelper.Encode(returnUrl.ToString()));
        }
    }
}
