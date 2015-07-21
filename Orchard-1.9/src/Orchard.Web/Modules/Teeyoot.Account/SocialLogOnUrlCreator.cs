using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using RM.QuickLogOn.OAuth.Models;

namespace Teeyoot.Account
{
    public static class SocialLogOnUrlCreator
    {
        public const string FacebookUrl =
            "https://www.facebook.com/dialog/oauth?client_id={0}&response_type=code&scope=email&redirect_uri={1}&state={2}";

        public static string GetFacebookLogOnUrl(this HtmlHelper htmlHelper, WorkContext workContext)
        {
            var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
            var facebookSettingsPart = workContext.CurrentSite.As<FacebookSettingsPart>();
            var clientId = facebookSettingsPart.ClientId;
            var returnUrl = workContext.HttpContext.Request.Url;
            var redirectUrl = new Uri(
                returnUrl,
                urlHelper.Action("FacebookAuth", "Account", new {Area = "Teeyoot.Account"})
                )
                .ToString();

            return string.Format(
                FacebookUrl,
                clientId,
                urlHelper.Encode(redirectUrl),
                urlHelper.Encode(returnUrl.ToString()));
        }
    }
}