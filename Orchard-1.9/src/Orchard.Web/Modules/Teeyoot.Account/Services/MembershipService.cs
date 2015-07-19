using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Security;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Users.Models;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Services
{
    /*
    [UsedImplicitly]
    public class MembershipService
    {
        // ReSharper disable once InconsistentNaming
        private const string PBKDF2 = "PBKDF2";

        private readonly IOrchardServices _orchardServices;
        private readonly IEncryptionService _encryptionService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;

        public MembershipService(
            IOrchardServices orchardServices,
            IEncryptionService encryptionService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository)
        {
            _orchardServices = orchardServices;
            _encryptionService = encryptionService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public MembershipSettings GetSettings()
        {
            var settings = new MembershipSettings();
            // accepting defaults
            return settings;
        }

        public IUser CreateUser(CreateUserParams createUserParams)
        {
            Logger.Information("CreateUser {0} {1}", createUserParams.Username, createUserParams.Email);

            var teeyootUser = _orchardServices.ContentManager.New("User");

            var userPart = teeyootUser.As<UserPart>();

            userPart.UserName = createUserParams.Username;
            userPart.Email = createUserParams.Email;
            userPart.NormalizedUserName = createUserParams.Username.ToLowerInvariant();
            userPart.HashAlgorithm = PBKDF2;
            //SetPassword(userPart, createUserParams.Password);

            
            var teeyootUserPart = teeyootUser.As<TeeyootUserPart>();

            teeyootUserPart.CreatedUtc = DateTime.UtcNow;
             

            _orchardServices.ContentManager.Create(teeyootUser);

            
            
            var role = _roleService.GetRoleByName("Administrator");

            if (role != null)
            {
                _userRolesRepository.Create(new UserRolesPartRecord
                {
                    UserId = teeyootUser.As<IUser>().Id,
                    Role = role
                });
            }
             

            //var teeyootUser = _orchardServices.ContentManager.New<TeeyootUserPart>("User");

            //teeyootUser.CreatedUtc = DateTime.UtcNow;

            //_orchardServices.ContentManager.Create(teeyootUser);

            return userPart;
        }

        public IUser GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public IUser ValidateUser(string userNameOrEmail, string password)
        {
            throw new NotImplementedException();
        }

        public void SetPassword(IUser user, string password)
        {
            if (!user.Is<UserPart>())
                throw new InvalidCastException();

            var userPart = user.As<UserPart>();

            switch (GetSettings().PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    SetPasswordClear(userPart, password);
                    break;
                case MembershipPasswordFormat.Hashed:
                    SetPasswordHashed(userPart, password);
                    break;
                case MembershipPasswordFormat.Encrypted:
                    SetPasswordEncrypted(userPart, password);
                    break;
                default:
                    throw new ApplicationException(T("Unexpected password format value").ToString());
            }
        }

        private static void SetPasswordClear(UserPart userPart, string password)
        {
            userPart.PasswordFormat = MembershipPasswordFormat.Clear;
            userPart.Password = password;
            userPart.PasswordSalt = null;
        }

        private static void SetPasswordHashed(UserPart userPart, string password)
        {
            var saltBytes = new byte[0x10];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetBytes(saltBytes);
            }

            userPart.PasswordFormat = MembershipPasswordFormat.Hashed;
            userPart.Password = ComputeHashBase64(userPart.HashAlgorithm, saltBytes, password);
            userPart.PasswordSalt = Convert.ToBase64String(saltBytes);
        }

        private static string ComputeHashBase64(string hashAlgorithmName, byte[] saltBytes, string password)
        {
            var combinedBytes = CombineSaltAndPassword(saltBytes, password);

            // Extending HashAlgorithm would be too complicated: http://stackoverflow.com/questions/6460711/adding-a-custom-hashalgorithmtype-in-c-sharp-asp-net?lq=1
            if (hashAlgorithmName == PBKDF2)
            {
                // HashPassword() already returns a base64 string.
                return Crypto.HashPassword(Encoding.Unicode.GetString(combinedBytes));
            }
            else
            {
                using (var hashAlgorithm = HashAlgorithm.Create(hashAlgorithmName))
                {
                    return Convert.ToBase64String(hashAlgorithm.ComputeHash(combinedBytes));
                }
            }
        }

        private static byte[] CombineSaltAndPassword(byte[] saltBytes, string password)
        {
            var passwordBytes = Encoding.Unicode.GetBytes(password);
            return saltBytes.Concat(passwordBytes).ToArray();
        }

        private void SetPasswordEncrypted(UserPart userPart, string password)
        {
            userPart.Password = Convert.ToBase64String(_encryptionService.Encode(Encoding.UTF8.GetBytes(password)));
            userPart.PasswordSalt = null;
            userPart.PasswordFormat = MembershipPasswordFormat.Encrypted;
        }
    }
     */
     
    
}