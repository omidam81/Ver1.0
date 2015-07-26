using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Models;
using Orchard.Users.Services;
using RM.QuickLogOn.OAuth.Services;
using RM.QuickLogOn.OAuth.ViewModels;
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
        private const string FacebookLogOnFailedErrorKey = "FacebookLogOnFailedError";

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

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private int MinPasswordLength
        {
            get { return _membershipService.GetSettings().MinRequiredPasswordLength; }
        }

        [HttpGet]
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

            if (TempData[FacebookLogOnFailedErrorKey] != null)
            {
                viewModel.FacebookLogOnFailed = true;
                viewModel.FacebookLogOnFailedError = (string) TempData[FacebookLogOnFailedErrorKey];
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(CreateAccountViewModel viewModel)
        {
            if (!ValidateRegistration(viewModel.Email, viewModel.Password, viewModel.ConfirmPassword))
            {
                return this.RedirectLocal("~/Login");
            }

            var user = _teeyootMembershipService.CreateUser(viewModel.Email, viewModel.Password);
            if (user == null)
            {
                return this.RedirectLocal("~/Login");
            }
            _authenticationService.SignIn(user, false);

            return this.RedirectLocal(!string.IsNullOrEmpty(viewModel.ReturnUrl) ? viewModel.ReturnUrl : "~/");
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel viewModel)
        {
            var user = ValidateLogOn(viewModel.Email, viewModel.Password);
            if (user == null)
            {
                return this.RedirectLocal("~/Login");
            }
            _authenticationService.SignIn(user, viewModel.RememberMe);

            return this.RedirectLocal(string.IsNullOrEmpty(viewModel.ReturnUrl) ? "~/" : viewModel.ReturnUrl);
        }

        public ActionResult FacebookAuth(FacebookOAuthAuthViewModel model)
        {
            var response = _facebookOAuthService.Auth(
                _workContextAccessor.GetContext(),
                model.Code,
                model.Error,
                model.State);

            if (response.Error == null)
            {
                return this.RedirectLocal("~/");
            }

            TempData[FacebookLogOnFailedErrorKey] = response.Error.ToString();
            return Redirect("~/Login");
        }

        public ActionResult GoogleAuth(GoogleOAuthAuthViewModel model)
        {
            var response = _googleOAuthService.Auth(
                _workContextAccessor.GetContext(),
                model.Code,
                model.Error,
                model.State);

            if (response.Error == null)
            {
                return this.RedirectLocal("~/");
            }

            TempData["GoogleLogOnFailedError"] = response.Error.ToString();
            return Redirect("~/Login");
        }

        private bool ValidateRegistration(string email, string password, string confirmPassword)
        {
            var validate = true;

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
                validate = false;
            }
            else if (email.Length >= 255)
            {
                emailIsTooLong = T("Email is too long").ToString();
                validate = false;
            }
            else if (!Regex.IsMatch(email, UserPart.EmailPattern, RegexOptions.IgnoreCase))
            {
                emailIsInvalid = T("Email is invalid").ToString();
                validate = false;
            }
            else if (!_userService.VerifyUserUnicity(email, email))
            {
                userAlreadyExists = T("A user already exists with the Email you have given").ToString();
                validate = false;
            }

            if (string.IsNullOrEmpty(password))
            {
                passwordCantBeBlank = T("Password can't be blank").ToString();
                validate = false;
            }
            else if (password.Length < MinPasswordLength)
            {
                passwordIsTooShort =
                    T("Password is too short (minimum is {0} characters)", MinPasswordLength).ToString();
                validate = false;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                confirmPasswordCantBeBlank = T("Password confirmation can't be blank").ToString();
                validate = false;
            }
            else if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                passwordDoesntMatch = T("Password confirmation doesn't match Password").ToString();
                validate = false;
            }

            var validationSummary = string.Join(". ", new[]
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

            if (!validate)
            {
                TempData[RegistrationValidationSummaryKey] = validationSummary;
            }

            return validate;
        }

        private IUser ValidateLogOn(string email, string password)
        {
            var validate = true;
            var validationSummary = T("Sorry, that is not a valid login!").ToString();

            if (string.IsNullOrEmpty(email))
            {
                validate = false;
            }

            if (string.IsNullOrEmpty(password))
            {
                validate = false;
            }

            var user = _membershipService.ValidateUser(email, password);
            if (user == null)
            {
                validate = false;
            }

            if (validate)
            {
                return user;
            }

            TempData[LoggingOnValidationSummaryKey] = validationSummary;
            return null;
        }
    }
}