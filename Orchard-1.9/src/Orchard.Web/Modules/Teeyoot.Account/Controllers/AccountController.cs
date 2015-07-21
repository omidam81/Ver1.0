using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Models;
using Orchard.Users.Services;
using Teeyoot.Account.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Controllers
{
    [HandleError]
    [Themed]
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IOrchardServices _orchardServices;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;

        private const string RegistrationValidationSummaryKey = "RegistrationValidationSummary";
        private const string LoggingOnValidationSummaryKey = "LoggingOnValidationSummary";

        public AccountController(
            IAuthenticationService authenticationService,
            IMembershipService membershipService,
            IUserService userService,
            IOrchardServices orchardServices,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository)
        {
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _userService = userService;
            _orchardServices = orchardServices;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private int MinPasswordLength
        {
            get { return _membershipService.GetSettings().MinRequiredPasswordLength; }
        }

        [HttpGet]
        [AlwaysAccessible]
        public ActionResult Index()
        {
            var viewModel = new AccountIndexViewModel
            {
                CreateAccountViewModel = new CreateAccountViewModel(),
                LogOnViewModel = new LogOnViewModel()
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

            return View(viewModel);
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult Register(CreateAccountViewModel viewModel, string returnUrl = null)
        {
            if (ValidateRegistration(viewModel.Email, viewModel.Password, viewModel.ConfirmPassword))
            {
                var user = CreateTeeyootUser(viewModel.Email, viewModel.Password);
                if (user != null)
                {
                    _authenticationService.SignIn(user, false);
                }
            }

            return Redirect("~/Login");
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult LogOn(LogOnViewModel viewModel, string returnUrl)
        {
            var user = ValidateLogOn(viewModel.Email, viewModel.Password);

            if (user != null)
            {
                _authenticationService.SignIn(user, viewModel.RememberMe);
            }

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
            const string validationSummary = "Sorry, that is not a valid login!";

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

            if (!validate)
            {
                TempData[LoggingOnValidationSummaryKey] = validationSummary;
                return user;
            }

            return null;
        }

        private IUser CreateTeeyootUser(string email, string password)
        {
            var registrationSettings = _orchardServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();

            var teeyootUser = _orchardServices.ContentManager.New("TeeyootUser");

            var userPart = teeyootUser.As<UserPart>();

            userPart.UserName = email;
            userPart.Email = email;
            userPart.NormalizedUserName = email.ToLowerInvariant();
            userPart.HashAlgorithm = "SHA1";
            _membershipService.SetPassword(userPart, password);

            if (registrationSettings != null)
            {
                userPart.RegistrationStatus = registrationSettings.UsersAreModerated
                    ? UserStatus.Pending
                    : UserStatus.Approved;
                userPart.EmailStatus = registrationSettings.UsersMustValidateEmail
                    ? UserStatus.Pending
                    : UserStatus.Approved;
            }

            var teeyootUserPart = teeyootUser.As<TeeyootUserPart>();

            teeyootUserPart.CreatedUtc = DateTime.UtcNow;

            _orchardServices.ContentManager.Create(teeyootUser);

            var role = _roleService.GetRoleByName("Seller");
            if (role != null)
            {
                _userRolesRepository.Create(new UserRolesPartRecord
                {
                    UserId = userPart.Id,
                    Role = role
                });
            }

            return userPart;
        }
    }
}