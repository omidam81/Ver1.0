using System;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Models;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Controllers
{
    [HandleError]
    [Themed]
    public class AccountController : Controller
    {
        // ReSharper disable once InconsistentNaming
        private const string PBKDF2 = "PBKDF2";

        private readonly IOrchardServices _orchardServices;
        private readonly IEncryptionService _encryptionService;

        public AccountController(
            IOrchardServices orchardServices,
            IEncryptionService encryptionService)
        {
            _orchardServices = orchardServices;
            _encryptionService = encryptionService;
        }

        public ActionResult Index()
        {
            return View("");
        }

        [HttpPost]
        [AlwaysAccessible]
        [ValidateInput(false)]
        public ActionResult Register(string email, string password, string confirmPassword)
        {
            CreateUser(email, password);

            return RedirectToAction("Index");
        }

        private void CreateUser(string email, string password)
        {
            var teeyootUser = _orchardServices.ContentManager.New("TeeyootUser");

            var userPart = teeyootUser.As<UserPart>();

            userPart.UserName = email;
            userPart.Email = email;
            userPart.NormalizedUserName = email.ToLowerInvariant();
            userPart.HashAlgorithm = PBKDF2;

            var teeyootUserPart = teeyootUser.As<TeeyootUserPart>();

            teeyootUserPart.CreatedUtc = DateTime.UtcNow;

            _orchardServices.ContentManager.Create(teeyootUser);
        }
    }
}