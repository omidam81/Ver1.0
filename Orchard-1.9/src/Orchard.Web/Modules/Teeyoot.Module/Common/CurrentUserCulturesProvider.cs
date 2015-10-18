using Orchard;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Services;
using Orchard.Localization;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Common.Utils;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Orchard.Localization.Records;
using RM.Localization;
using Teeyoot.Localization;

namespace Teeyoot.Module.Common
{
    // Created especcialy for RM.Localization.CookieCulturePickerDriver 
    // to pass there the list of the cultures for the current user.
    public class CurrentUserCulturesProvider : ICurrentUserCulturesProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IRepository<CultureRecord> _cultureRepository;
        private readonly IRepository<LinkCountryCultureRecord> _linkCountryCultureRepository;
        private readonly IRepository<TeeyootUserPartRecord> _teeyootUserPartRepository;

        public CurrentUserCulturesProvider(
            IWorkContextAccessor workContextAccessor, 
            IRepository<CountryRecord> countryRepository,
            IRepository<CultureRecord> cultureRepository,
            IRepository<LinkCountryCultureRecord> linkCountryCultureRepository,
            IRepository<TeeyootUserPartRecord> teeyootUserPartRepository
            )
        {
            _workContextAccessor = workContextAccessor;
            _countryRepository = countryRepository;
            _cultureRepository = cultureRepository;
            _linkCountryCultureRepository = linkCountryCultureRepository;
            _teeyootUserPartRepository = teeyootUserPartRepository;
        }


        /// <summary>
        /// Returns the list of the cultures for the current user (whether seller or buyer) 
        /// </summary>
        /// <returns></returns>
        List<string> ICurrentUserCulturesProvider.GetCulturesForCurrentUser()
        {
            CountryRecord userCountry;

            var orchardUser = _workContextAccessor.GetContext().CurrentUser;
            if (null != orchardUser)
            {
                var teeyootUser = _teeyootUserPartRepository.Get(orchardUser.Id);
                userCountry = teeyootUser.CountryRecord;
            }
            else
            // If this is a buyer.
            {
                var localInfo = LocalizationInfoFactory.GetCurrentLocalizationInfo();
                userCountry = _countryRepository.Table.Where(c => c.Code == localInfo.CountryIsoCode).FirstOrDefault();
                // If a buyer is from the other country that does'n belong to our country list
                //  we must treat his country as Malasiya by default.
                if (userCountry == null)
                {
                    userCountry = _countryRepository.Table.Where(c => c.Code.ToUpper() == "MY").First();
                }
            }
            var cultures = _linkCountryCultureRepository.Table.Where(l => l.CountryRecord.Id == userCountry.Id).Select(l => l.CultureRecord.Culture).ToList();
            return cultures;
        }
    }
}