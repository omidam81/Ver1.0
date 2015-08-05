using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Models;
using Orchard.Users.Services;
using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.OAuth.ViewModels;
using Teeyoot.Account.Common;
using Teeyoot.Account.DTOs;
using Teeyoot.Account.Services;
using Teeyoot.Account.ViewModels;

namespace Teeyoot.Account.Controllers
{
    [HandleError]
    [Themed]
    public class AccountController : Controller
    {
        private readonly ITeeyootMembershipService _teeyootMembershipService;
        private readonly IFacebookOAuthService _facebookOAuthService;
        private readonly IGoogleOAuthService _googleOAuthService;

        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IWorkContextAccessor _workContextAccessor;

        private const string RegistrationValidationSummaryKey = "RegistrationValidationSummary";
        private const string LoggingOnValidationSummaryKey = "LoggingOnValidationSummary";
        private const string RecoverValidationSummaryKey = "RecoverValidationSummary";
        private const string RecoverEmailSentKey = "RecoverEmailSent";
        private const string ResetPasswordValidationSummaryKey = "ResetPasswordValidationSummary";
        private const string PasswordHasBeenUpdatedKey = "PasswordHasBeenUpdated";

        public AccountController(
            ITeeyootMembershipService teeyootMembershipService,
            IAuthenticationService authenticationService,
            IMembershipService membershipService,
            IUserService userService,
            IFacebookOAuthService facebookOAuthService,
            IGoogleOAuthService googleOAuthService,
            IWorkContextAccessor workContextAccessor)
        {
            _teeyootMembershipService = teeyootMembershipService;
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userService = userService;
            _facebookOAuthService = facebookOAuthService;
            _googleOAuthService = googleOAuthService;
            _workContextAccessor = workContextAccessor;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        private int MinPasswordLength
        {
            get { return _membershipService.GetSettings().MinRequiredPasswordLength; }
        }

        public ActionResult Index(string logOnReturnUrl, string registerReturnUrl)
        {
            var viewModel = new AccountIndexViewModel
            {
                CreateAccountViewModel = new CreateAccountViewModel(registerReturnUrl),
                LogOnViewModel = new LogOnViewModel(logOnReturnUrl),
            };

            if (TempData[RegistrationValidationSummaryKey] != null)
            {
                viewModel.RegistrationValidationIssueOccurred = true;
                viewModel.RegistrationValidationSummary = (string) TempData[RegistrationValidationSummaryKey];
            }

            if (TempData[LoggingOnValidationSummaryKey] != null)
            {
                viewModel.LoggingOnValidationIssueOccurred = true;
                viewModel.LoggingOnValidationSummary = (string) TempData[LoggingOnValidationSummaryKey];
            }

            if (TempData[PasswordHasBeenUpdatedKey] != null)
            {
                viewModel.PasswordHasBeenUpdated = (bool) TempData[PasswordHasBeenUpdatedKey];
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(CreateAccountViewModel viewModel)
        {
            var validRes = ValidateRegistration(viewModel.Email, viewModel.Password, viewModel.ConfirmPassword);
            if (!validRes.IsValid)
            {
                TempData[RegistrationValidationSummaryKey] = validRes.ValidationSummary;
                return this.RedirectLocal("~/Login");
            }

            var user = _teeyootMembershipService.CreateUser(viewModel.Email, viewModel.Password);
            if (user == null)
            {
                return this.RedirectLocal("~/Login");
            }

            _authenticationService.SignIn(user, false);

            return string.IsNullOrEmpty(viewModel.ReturnUrl)
                ? Redirect("~/")
                : this.RedirectLocal(viewModel.ReturnUrl);
        }

        [HttpPost]
        public JsonResult WizardRegister(WizardRegisterJsonRequest request)
        {
            var validRes = ValidateRegistration(request.Email, request.Password, request.ConfirmPassword);
            if (!validRes.IsValid)
            {
                var response = new WizardRegisterJsonResponse
                {
                    IssueOccurred = true,
                    IssueSummary = validRes.ValidationSummary
                };

                return Json(response);
            }

            var user = _teeyootMembershipService.CreateUser(request.Email, request.Password);
            if (user == null)
            {
                var response = new WizardRegisterJsonResponse
                {
                    IssueOccurred = true,
                    IssueSummary = T("Registration issue occurred.").ToString()
                };

                return Json(response);
            }

            _authenticationService.SignIn(user, false);

            return Json(new WizardRegisterJsonResponse {IssueSummary = "Success"});
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel viewModel)
        {
            var validRes = ValidateLogOn(viewModel.Email, viewModel.Password);
            if (!validRes.IsValid)
            {
                TempData[LoggingOnValidationSummaryKey] = validRes.ValidationSummary;
                return this.RedirectLocal("~/Login");
            }

            _authenticationService.SignIn(validRes.User, viewModel.RememberMe);

            return string.IsNullOrEmpty(viewModel.ReturnUrl)
                ? Redirect("~/")
                : this.RedirectLocal(viewModel.ReturnUrl);
        }

        [HttpPost]
        public JsonResult WizardLogOn(WizardLogOnJsonRequest request)
        {
            var validRes = ValidateLogOn(request.Email, request.Password);
            if (!validRes.IsValid)
            {
                var response = new WizardLogOnJsonResponse
                {
                    IssueOccurred = true,
                    IssueSummary = validRes.ValidationSummary
                };

                return Json(response);
            }

            _authenticationService.SignIn(validRes.User, request.RememberMe);

            return Json(new WizardLogOnJsonResponse());
        }

        public ActionResult FacebookAuth(FacebookOAuthAuthViewModel model)
        {
            var response = _facebookOAuthService.Auth(
                _workContextAccessor.GetContext(),
                model.Code,
                model.Error,
                model.State);

            return Redirect(response.Error == null ? "~/" : "~/Login");
        }

        public ActionResult GoogleAuth(GoogleOAuthAuthViewModel model)
        {
            var response = _googleOAuthService.Auth(
                _workContextAccessor.GetContext(),
                model.Code,
                model.Error,
                model.State);

            return Redirect(response.Error == null ? "~/" : "~/Login");
        }

        public ActionResult Recover()
        {
            var viewModel = new RecoverViewModel();

            if (TempData[RecoverValidationSummaryKey] != null)
            {
                viewModel.RecoverFailed = true;
                viewModel.RecoverIssueSummary = (string) TempData[RecoverValidationSummaryKey];
            }

            if (TempData[RecoverEmailSentKey] != null)
            {
                viewModel.RecoverEmailSent = (bool) TempData[RecoverEmailSentKey];
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Recover(RecoverViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Email))
            {
                TempData[RecoverValidationSummaryKey] = T("You did not provide a valid email.").ToString();
            }
            else
            {
                var siteUrl = _workContextAccessor.GetContext().HttpContext.Request.Url;
                _userService.SendLostPasswordEmail(viewModel.Email, nonce =>
                    new Uri(siteUrl, Url.Action("ResetPassword", "Account", new {area = "Teeyoot.Account", nonce}))
                        .ToString());

                TempData[RecoverEmailSentKey] = true;
            }

            return this.RedirectLocal("~/Recover");
        }

        public ActionResult ResetPassword(string nonce)
        {
            var viewModel = new ResetPasswordViewModel();

            var user = _userService.ValidateLostPassword(nonce);
            if (user == null)
            {
                return this.RedirectLocal("~/Login");
            }

            if (TempData[ResetPasswordValidationSummaryKey] != null)
            {
                viewModel.ResetPasswordIssueOccurred = true;
                viewModel.ResetPasswordValidationSummary = (string) TempData[ResetPasswordValidationSummaryKey];
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel viewModel)
        {
            var user = _userService.ValidateLostPassword(viewModel.Nonce);
            if (user == null)
            {
                return Redirect("~/");
            }

            var validRes = ValidateNewPassword(viewModel.Password, viewModel.ConfirmPassword);
            if (!validRes.IsValid)
            {
                TempData[ResetPasswordValidationSummaryKey] = validRes.ValidationSummary;
                return this.RedirectLocal(Url.Action("ResetPassword", "Account", new {nonce = viewModel.Nonce}));
            }

            _membershipService.SetPassword(user, viewModel.Password);

            TempData[PasswordHasBeenUpdatedKey] = true;
            return this.RedirectLocal("~/Login");
        }

        public ActionResult RefreshToken()
        {
            return PartialView("AntiForgeryTokenValue");
        }

        private ValidateRegistrationResult ValidateRegistration(string email, string password, string confirmPassword)
        {
            var res = new ValidateRegistrationResult {IsValid = true};

            string emailCantBeBlank = null;
            string emailIsTooLong = null;
            string emailIsInvalid = null;
            string userAlreadyExists = null;
            string passwordCantBeBlank = null;
            string passwordIsTooShort = null;
            string passwordDoesntMatch = null;
            string confirmPasswordCantBeBlank = null;

            if (string.IsNullOrEmpty(email))
            {
                emailCantBeBlank = T("Email can't be blank").ToString();
                res.IsValid = false;
            }
            else if (email.Length >= 255)
            {
                emailIsTooLong = T("Email is too long").ToString();
                res.IsValid = false;
            }
            else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase))
            {
                emailIsInvalid = T("Email is invalid").ToString();
                res.IsValid = false;
            }
            else if (!_userService.VerifyUserUnicity(email, email))
            {
                userAlreadyExists = T("A user already exists with the Email you have given").ToString();
                res.IsValid = false;
            }

            if (string.IsNullOrEmpty(password))
            {
                passwordCantBeBlank = T("Password can't be blank").ToString();
                res.IsValid = false;
            }
            else if (password.Length < MinPasswordLength)
            {
                passwordIsTooShort =
                    T("Password is too short (minimum is {0} characters)", MinPasswordLength).ToString();
                res.IsValid = false;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                confirmPasswordCantBeBlank = T("Password confirmation can't be blank").ToString();
                res.IsValid = false;
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                passwordDoesntMatch = T("Password confirmation doesn't match Password").ToString();
                res.IsValid = false;
            }

            res.ValidationSummary = string.Join(". ", new[]
            {
                emailCantBeBlank,
                emailIsTooLong,
                emailIsInvalid,
                userAlreadyExists,
                passwordCantBeBlank,
                passwordIsTooShort,
                confirmPasswordCantBeBlank,
                passwordDoesntMatch
            }.Where(it => it != null));

            return res;
        }

        private ValidateLogOnResult ValidateLogOn(string email, string password)
        {
            var res = new ValidateLogOnResult
            {
                IsValid = false,
                ValidationSummary = T("Sorry, that is not a valid login!").ToString()
            };

            if (string.IsNullOrEmpty(email))
            {
                return res;
            }

            if (string.IsNullOrEmpty(password))
            {
                return res;
            }

            var user = _membershipService.ValidateUser(email, password);
            if (user == null)
            {
                return res;
            }

            return new ValidateLogOnResult
            {
                IsValid = true,
                User = user
            };
        }

        private ValidateNewPasswordResult ValidateNewPassword(string password, string confirmPassword)
        {
            var res = new ValidateNewPasswordResult {IsValid = true};

            string passwordCantBeBlank = null;
            string passwordIsTooShort = null;
            string passwordDoesntMatch = null;
            string confirmPasswordCantBeBlank = null;

            if (string.IsNullOrEmpty(password))
            {
                passwordCantBeBlank = T("Password can't be blank").ToString();
                res.IsValid = false;
            }
            else if (password.Length < MinPasswordLength)
            {
                passwordIsTooShort =
                    T("Password is too short (minimum is {0} characters)", MinPasswordLength).ToString();
                res.IsValid = false;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                confirmPasswordCantBeBlank = T("Password confirmation can't be blank").ToString();
                res.IsValid = false;
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                passwordDoesntMatch = T("Password confirmation doesn't match Password").ToString();
                res.IsValid = false;
            }

            res.ValidationSummary = string.Join(". ", new[]
            {
                passwordCantBeBlank,
                passwordIsTooShort,
                confirmPasswordCantBeBlank,
                passwordDoesntMatch
            }.Where(it => it != null));

            return res;
        }
    }
}