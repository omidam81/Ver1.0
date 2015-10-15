﻿using System;
using System.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Users.Models;
using Teeyoot.Localization;
using Teeyoot.Module.Common;
using Teeyoot.Module.Models;

namespace Teeyoot.Account.Services
{
    public class TeeyootMembershipService : ITeeyootMembershipService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMembershipService _membershipService;
        private readonly IRoleService _roleService;
        private readonly IRepository<UserRolesPartRecord> _userRolesRepository;
        private readonly IRepository<CurrencyRecord> _currencyRecordRepository;
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IWorkContextAccessor _workContextAccessor;

        // ReSharper disable once InconsistentNaming
        private const string PBKDF2 = "PBKDF2";

        public ILogger Logger { get; set; }

        public TeeyootMembershipService(
            IOrchardServices orchardServices,
            IMembershipService membershipService,
            IRoleService roleService,
            IRepository<UserRolesPartRecord> userRolesRepository,
            IRepository<CurrencyRecord> currencyRecordRepository,
            IRepository<CountryRecord> countryRepository,
            IWorkContextAccessor workContextAccessor)
        {
            _orchardServices = orchardServices;
            _membershipService = membershipService;
            _roleService = roleService;
            _userRolesRepository = userRolesRepository;
            _currencyRecordRepository = currencyRecordRepository;
            _countryRepository = countryRepository;

            Logger = NullLogger.Instance;
            _workContextAccessor = workContextAccessor;
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
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            var cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");

            teeyootUserPart.CreatedUtc = DateTime.UtcNow;
            teeyootUserPart.PhoneNumber = phone;
            teeyootUserPart.PublicName = name;
            teeyootUserPart.TeeyootUserCulture = cultureUsed;

            var localizationInfo = LocalizationInfoFactory.GetCurrentLocalizationInfo();

            var currencyCode = CountryCurrencyHelper.GetCountryCurrencyCode(localizationInfo.Country);
            var currency = _currencyRecordRepository.Table
                .FirstOrDefault(c => c.CurrencyCulture == cultureUsed && c.Code == currencyCode);

            if (currency == null)
                throw new ApplicationException("Currency is not found in database");

            teeyootUserPart.CurrencyId = currency.Id;

            var country = _countryRepository.Table
                .FirstOrDefault(c => c.Code == localizationInfo.CountryIsoCode);

            teeyootUserPart.CountryRecord = country ?? _countryRepository.Table.First(c => c.Code == "US");

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