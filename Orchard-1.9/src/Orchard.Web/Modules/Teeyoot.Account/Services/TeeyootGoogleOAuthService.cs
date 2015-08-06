using System;
using System.IO;
using System.Net;
using System.Text;
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
    public class TeeyootGoogleOAuthService : ITeeyootGoogleOAuthService
    {
        public const string TokenRequestUrl = "https://accounts.google.com/o/oauth2/token";
        public const string EmailRequestUrl = "https://www.googleapis.com/oauth2/v1/userinfo?access_token={0}";
        public const string VerifyTokenUrl = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}";

        private readonly ITeeyootSocialLogOnService _teeyootSocialLogOnService;
        private readonly IEncryptionService _oauthHelper;
        private readonly IWorkContextAccessor _workContextAccessor;

        public WorkContext WorkContext
        {
            get { return _workContextAccessor.GetContext(); }
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public TeeyootGoogleOAuthService(
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

        private string GetAccessToken(WorkContext workContext, string code)
        {
            try
            {
                var part = workContext.CurrentSite.As<GoogleSettingsPart>();
                var clientId = part.ClientId;
                var clientSecret = _oauthHelper.Decrypt(part.Record.EncryptedClientSecret);

                var urlHelper = new UrlHelper(workContext.HttpContext.Request.RequestContext);
                var redirectUrl = new Uri(
                    workContext.HttpContext.Request.Url,
                    urlHelper.Action("GoogleAuth", "Account", new {area = "Teeyoot.Account"})
                    ).ToString();

                var wr = WebRequest.Create(TokenRequestUrl);
                wr.Proxy = OAuthHelper.GetProxy();
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                using (var stream = wr.GetRequestStream())
                using (var ws = new StreamWriter(stream, Encoding.UTF8))
                {
                    ws.Write("code={0}&", code);
                    ws.Write("client_id={0}&", clientId);
                    ws.Write("client_secret={0}&", clientSecret);
                    ws.Write("redirect_uri={0}&", redirectUrl);
                    ws.Write("grant_type=authorization_code");
                }
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<GoogleAccessTokenJsonModel>(stream);
                    return result.access_token;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return null;
        }

        private string GetEmailAddress(string token)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(EmailRequestUrl, token));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<GoogleEmailAddressJsonViewModel>(stream);
                    return result != null && result.verified_email ? result.email : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return null;
        }

        public QuickLogOnResponse Auth(WorkContext workContext, string code, string error, string returnUrl)
        {
            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(error))
            {
                error = T("LogOn through Google failed").ToString();
            }
            else
            {
                var token = GetAccessToken(workContext, code);
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
                            "Email address required. Please make sure your Google profile includes an email and try again.")
                            .ToString();
                }
                else
                {
                    error = T("LogOn through Google failed").ToString();
                }
            }
            return new QuickLogOnResponse
            {
                Error = T(error),
                ReturnUrl = returnUrl
            };
        }

        public bool VerifyToken(string tokenToVerify)
        {
            try
            {
                var wr = WebRequest.Create(string.Format(VerifyTokenUrl, tokenToVerify));
                wr.Method = "GET";
                wr.Proxy = OAuthHelper.GetProxy();
                var wres = wr.GetResponse();
                using (var stream = wres.GetResponseStream())
                {
                    var result = OAuthHelper.FromJson<GoogleTokenVerifyingJsonViewModel>(stream);
                    var part = WorkContext.CurrentSite.As<GoogleSettingsPart>();
                    if (result.audience == part.ClientId)
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

        public QuickLogOnResponse WizardAuth(string tokenToVerify)
        {
            LocalizedString error;

            if (string.IsNullOrEmpty(tokenToVerify))
            {
                error = T("LogOn through Google failed");
            }
            else
            {
                var tokenVerification = VerifyToken(tokenToVerify);
                if (tokenVerification)
                {
                    var email = GetEmailAddress(tokenToVerify);
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
                            "Email address required. Please make sure your Google profile includes an email and try again.");
                }
                else
                {
                    error = T("LogOn through Google failed");
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