using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Data;
using Orchard.Roles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Drivers
{
    public class AllCountryWidget: ContentPartDriver<AllCountryWidgetPart>
    {
        private readonly ICountryService _countryService;
        private readonly IOrchardServices _orchardServices;
        private readonly IWorkContextAccessor _wca;
        private readonly IRepository<UserRolesPartRecord> _userRolesPartRepository;

        public AllCountryWidget(ICountryService countryService, IOrchardServices orchardServices, IWorkContextAccessor wca, IRepository<UserRolesPartRecord> userRolesPartRepository)
        {
            _countryService = countryService;
            _orchardServices = orchardServices;
            _wca = wca;
            _userRolesPartRepository = userRolesPartRepository;
        }

        protected override DriverResult Display(AllCountryWidgetPart part, string displayType, dynamic shapeHelper)
        {
            var userIds = _userRolesPartRepository.Table.Where(x => x.Role.Name == "Administrator").Select(x => x.UserId).ToList();

            if (userIds.Contains(_wca.GetContext().CurrentUser.Id))
            {
                return ContentShape("Parts_AllCountryWidget", () => shapeHelper.Parts_AllCountryWidget(Providers: _countryService.GetAllCountry().ToList()));
            }

            return null;
        }
    }
}