using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Users.Models;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Services
{
    public class TeeyootMembershipService : ITeeyootMembershipService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;

        // ReSharper disable once InconsistentNaming
        private const string PBKDF2 = "PBKDF2";

        public ILogger Logger { get; set; }

        public TeeyootMembershipService(
            IOrchardServices orchardServices,
            IMembershipService membershipService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;

            Logger = NullLogger.Instance;
        }

        public IUser CreateUser(string email, string password, string name, string phone)
        {
            Logger.Information("CreateUser {0} {1}", email, password, name, phone);

            var teeyootUser = _orchardServices.ContentManager.New("TeeyootUser");

            var userPart = teeyootUser.As<UserPart>();

            userPart.UserName = email; 
            userPart.Email = email;
            userPart.NormalizedUserName = email.ToLowerInvariant();
            userPart.HashAlgorithm = PBKDF2;
            _membershipService.SetPassword(userPart, password);
            userPart.RegistrationStatus = UserStatus.Approved;
            userPart.EmailStatus = UserStatus.Approved;

            var teeyootUserPart = teeyootUser.As<TeeyootUserPart>();

            teeyootUserPart.CreatedUtc = DateTime.UtcNow;
            teeyootUserPart.PhoneNumber = phone;
            teeyootUserPart.PublicName = name;

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