using System;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Models;
using RM.QuickLogOn.Providers;

namespace Teeyoot.Account.Services
{
    public class TeeyootSocialLogOnService : ITeeyootSocialLogOnService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthenticationService _authenticationService;
        private readonly ITeeyootMembershipService _teeyootMembershipService;
        private readonly ITeeyootUserService _teeyootUserService;

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public TeeyootSocialLogOnService(
            IAuthenticationService authenticationService,
            IOrchardServices orchardServices,
            ITeeyootMembershipService teeyootMembershipService,
            ITeeyootUserService teeyootUserService)
        {
            _authenticationService = authenticationService;
            _orchardServices = orchardServices;
            _teeyootMembershipService = teeyootMembershipService;
            _teeyootUserService = teeyootUserService;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public QuickLogOnResponse LogOn(QuickLogOnRequest request)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser != null)
            {
                _authenticationService.SignOut();
            }

            var lowerEmail = request.Email.ToLowerInvariant();

            var user = _orchardServices.ContentManager.Query<UserPart, UserPartRecord>()
                .Where(u => u.NormalizedUserName == lowerEmail)
                .List()
                .FirstOrDefault();

            if (user == null)
            {
                user = _teeyootMembershipService.CreateUser(lowerEmail, Guid.NewGuid().ToString(), request.Email, null).As<UserPart>();
                if (user == null)
                {
                    return new QuickLogOnResponse
                    {
                        User = null,
                        Error = T("User can not be created to assign to Quick LogOn credentials")
                    };
                }
                _teeyootUserService.SendWelcomeEmail(user);
            }

            _authenticationService.SignIn(user, request.RememberMe);

            return new QuickLogOnResponse {User = user, ReturnUrl = request.ReturnUrl};
        }
    }
}