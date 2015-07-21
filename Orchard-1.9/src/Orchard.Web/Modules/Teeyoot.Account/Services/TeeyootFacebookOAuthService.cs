using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using RM.QuickLogOn.OAuth.Models;
using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.OAuth.ViewModels;
using RM.QuickLogOn.Providers;

namespace Teeyoot.Account.Services
{
    public class TeeyootFacebookOAuthService : IFacebookOAuthService
    {
        public const string TokenRequestUrl =
            "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

        public const string EmailRequestUrl = "https://graph.facebook.com/me?access_token={0}";

        private readonly ITeeyootSocialLogOnService _teeyootSocialLogOnService;
        private readonly IEncryptionService _oauthHelper;

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TeeyootFacebookOAuthService(IEncryptionService oauthHelper,
            ITeeyootSocialLogOnService teeyootSocialLogOnService)
        {
            _teeyootSocialLogOnService = teeyootSocialLogOnService;
            _oauthHelper = oauthHelper;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public string GetAccessToken(WorkContext wc, string code, string returnUrl)
        {
            try
            {
                var part = wc.CurrentSite.As<FacebookSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
                var redirectUrl = new Uri(
                    wc.HttpContext.Request.Url,
                    urlHelper.Action("FacebookAuth", "Account", new {Area = "Teeyoot.Account"})
                    ).ToString();
                var url = string.Format(TokenRequestUrl, urlHelper.Encode(clientId), urlHelper.Encode(redirectUrl),
                    urlHelper.Encode(clientSecret), urlHelper.Encode(code));
                var wr = WebRequest.Create(url);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.Method = "GET";
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var result = HttpUtility.ParseQueryString(sr.ReadToEnd());
                    return result["access_token"];
                }
            }
            catch (Exception ex)
            {
                string error = OAuthHelper.ReadWebExceptionMessage(ex);
                Logger.Error(ex, string.IsNullOrEmpty(error) ? ex.Message : error);
            }

            return null;
        }

        public string GetEmailAddress(string token)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(EmailRequestUrl, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<FacebookEmailAddressJsonViewModel>(stream);
                    return result != null && result.verified ? result.email : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }
            return null;
        }

        public string GetAccessTokenUrl(WorkContext wc, string code, string error, string returnUrl)
        {
            var part = wc.CurrentSite.As<FacebookSettingsPart>();
            var clientId = part.ClientId;
            var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

            var urlHelper = new UrlHelper(wc.HttpContext.Request.RequestContext);
            var redirectUrl = new Uri(
                wc.HttpContext.Request.Url,
                urlHelper.Action("FacebookAuth", "Account", new {Area = "Teeyoot.Account", returnUrl = returnUrl})
                ).ToString();

            return string.Format(
                TokenRequestUrl,
                clientId,
                urlHelper.Encode(redirectUrl),
                clientSecret,
                code);
        }

        public QuickLogOnResponse Auth(WorkContext workContext, string code, string error, string returnUrl)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = T("LogOn through Facebook failed").ToString();
            }
            else
            {
                var token = GetAccessToken(workContext, code, returnUrl);
                if (!string.IsNullOrEmpty(token))
                {
                    var email = GetEmailAddress(token);
                    if (!string.IsNullOrEmpty(email))
                    {
                        return _teeyootSocialLogOnService.LogOn(new QuickLogOnRequest
                        {
                            Email = email,
                            Login = email,
                            RememberMe = false,
                            ReturnUrl = returnUrl
                        });
                    }
                    error =
                        T(
                            "Email address required. Please make sure your Facebook profile includes an email and try again.")
                            .ToString();
                }
                else
                {
                    error = T("LogOn through Facebook failed").ToString();
                }
            }
            return new QuickLogOnResponse
            {
                Error = T(error),
                ReturnUrl = returnUrl
            };
        }
    }
}