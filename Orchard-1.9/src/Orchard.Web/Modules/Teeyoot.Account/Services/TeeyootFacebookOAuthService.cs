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
using Teeyoot.Account.Common;

namespace Teeyoot.Account.Services
{
    public class TeeyootFacebookOAuthService : ITeeyootFacebookOAuthService
    {
        public const string TokenRequestUrl =
            "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}&scope=email";

        public const string TokenRequestUrl1 =
            "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials";

        public const string EmailRequestUrl =
            "https://graph.facebook.com/v2.4/me?access_token={0}&fields=email,verified";

        public const string InspectTokenUrl =
            "https://graph.facebook.com/v2.4/debug_token?input_token={0}&access_token={1}";

        private readonly ITeeyootSocialLogOnService _teeyootSocialLogOnService;
        private readonly IEncryptionService _oauthHelper;
        private readonly IWorkContextAccessor _workContextAccessor;

        public WorkContext WorkContext
        {
            get { return _workContextAccessor.GetContext(); }
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TeeyootFacebookOAuthService(
            IEncryptionService oauthHelper,
            ITeeyootSocialLogOnService teeyootSocialLogOnService,
            IWorkContextAccessor workContextAccessor)
        {
            _teeyootSocialLogOnService = teeyootSocialLogOnService;
            _oauthHelper = oauthHelper;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public string GetAccessToken(WorkContext workContext, string code, string returnUrl)
        {
            try
            {
                var part = workContext.CurrentSite.As<FacebookSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
                var redirectUrl = new Uri(
                    workContext.HttpContext.Request.Url,
                    urlHelper.Action("FacebookAuth", "Account", new {area = "Teeyoot.Account"})
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

        public string GetAccessToken()
        {
            try
            {
                var part = WorkContext.CurrentSite.As<FacebookSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(WorkContext.HttpContext.Request.RequestContext);
                var url = string.Format(TokenRequestUrl1, urlHelper.Encode(clientId), urlHelper.Encode(clientSecret));

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
                urlHelper.Action("FacebookAuth", "Account", new {area = "Teeyoot.Account", returnUrl})
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

        public bool InspectToken(string inputToken, string accessToken)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(InspectTokenUrl, inputToken, accessToken));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<FacebookTokenInspectionJsonViewModel>(stream);
                    var part = WorkContext.CurrentSite.As<FacebookSettingsPart>();
                    if (result.data.app_id == part.ClientId)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return false;
        }

        public QuickLogOnResponse WizardAuth(string tokenToInspect)
        {
            LocalizedString error;

            if (string.IsNullOrEmpty(tokenToInspect))
            {
                error = T("LogOn through Facebook failed");
            }
            else
            {
                var token = GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var tokenInspection = InspectToken(tokenToInspect, token);
                    if (tokenInspection)
                    {
                        var email = GetEmailAddress(tokenToInspect);
                        if (!string.IsNullOrEmpty(email))
                        {
                            return _teeyootSocialLogOnService.LogOn(new QuickLogOnRequest
                            {
                                Email = email,
                                Login = email,
                                RememberMe = false,
                                ReturnUrl = ""
                            });
                        }
                        error =
                            T(
                                "Email address required. Please make sure your Facebook profile includes an email and try again.");

                    }
                    else
                    {
                        error = T("LogOn through Facebook failed");
                    }
                }
                else
                {
                    error = T("LogOn through Facebook failed");
                }
            }

            return new QuickLogOnResponse
            {
                Error = error,
                ReturnUrl = ""
            };
        }
    }
}