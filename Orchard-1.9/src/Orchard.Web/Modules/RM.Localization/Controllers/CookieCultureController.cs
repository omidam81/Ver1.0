using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Orchard.Themes;
using Orchard.Data;
using RM.Localization.Services;
using Orchard.Users.Models;
using Orchard;
using Orchard.Localization.Records;

namespace RM.Localization.Controllers
{
    [HandleError, Themed]
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class CookieCultureController : Controller
    {


        private readonly IOrchardServices _orchardServices;
        private IRepository<UserPartRecord> _userPartRepository;
        private IRepository<CultureRecord> _cultureRepository;

        private readonly ICookieCultureService _cookieCultureService;
        public CookieCultureController(ICookieCultureService cookieCultureService,
            IOrchardServices orchardServices,
            IRepository<UserPartRecord> userPartRepository,
            IRepository<CultureRecord> cultureRepository)
        {
            _cookieCultureService = cookieCultureService;

            _userPartRepository = userPartRepository;
            _orchardServices = orchardServices;
            _cultureRepository = cultureRepository;
        }

        [HttpGet]
        public ActionResult SetCulture(string culture, string returnUrl) {

            var currentUser = _orchardServices.WorkContext.CurrentUser;
            if (currentUser != null)
            {
                var userRecord = _userPartRepository.Get(currentUser.Id);
                userRecord.CultureRecord = _cultureRepository.Table.Where(c => c.Culture == culture).First();
                _userPartRepository.Update(userRecord);
            }
            _cookieCultureService.SetCulture(culture);
            return this.RedirectLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult ResetCulture(string returnUrl)
        {
            _cookieCultureService.ResetCulture();
            return this.RedirectLocal(returnUrl);
        }
    }
}
