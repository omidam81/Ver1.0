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

        public const string GoogleUrl =
            "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id={0}&redirect_uri={1}&scope={2}&state={3}";

        public const string Scope = "https://www.googleapis.com/auth/userinfo.email";

        public static string GetFacebookLogOnUrl(this HtmlHelper htmlHelper, WorkContext workContext)
        {
            var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
            var facebookSettingsPart = workContext.CurrentSite.As<FacebookSettingsPart>();
            var clientId = facebookSettingsPart.ClientId;
            var returnUrl = workContext.HttpContext.Request.Url;
            var redirectUrl = new Uri(
                returnUrl,
                urlHelper.Action("FacebookAuth", "Account", new {area = "Teeyoot.Account"})
                ).ToString();

            return string.Format(
                FacebookUrl,
                clientId,
                urlHelper.Encode(redirectUrl),
                urlHelper.Encode(returnUrl.ToString()));
        }

        public static string GetGoogleLogOnUrl(this HtmlHelper htmlHelper, WorkContext workContext)
        {
            var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
            var part = workContext.CurrentSite.As<GoogleSettingsPart>();
            var clientId = part.ClientId;
            var returnUrl = workContext.HttpContext.Request.Url;
            var redirectUrl = new Uri(
                returnUrl,
                urlHelper.Action("GoogleAuth", "Account", new {area = "Teeyoot.Account"})
                ).ToString();
            return string.Format(
                GoogleUrl,
                clientId,
                urlHelper.Encode(redirectUrl),
                urlHelper.Encode(Scope),
                urlHelper.Encode(returnUrl.ToString()));
        }
    }
}