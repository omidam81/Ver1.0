using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;
using Teeyoot.Module.Services.Interfaces;
using Orchard.Localization;
using Orchard;
using System.Globalization;
using Teeyoot.Module.Services;
using Orchard.Data;
using Orchard.ContentManagement;
using Orchard.Users.Models;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminRelaunchCampController : Controller
    {
        private readonly IRepository<BringBackCampaignRecord> _backCampaignRepository;
        private readonly IRepository<CampaignRecord> _campaignRepository;
        private readonly IContentManager _contentManager;
        private readonly ICampaignService _campaignService;
        private readonly IRepository<UserPartRecord> _userRepository;
        public Localizer T { get; set; }
        private IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;

        public AdminRelaunchCampController(IRepository<BringBackCampaignRecord> backCampaignRepository,IContentManager contentManager, IRepository<UserPartRecord> userRepository, IRepository<CampaignRecord> campaignRepository, ICampaignService campaignService, IOrchardServices services, IWorkContextAccessor workContextAccessor)
        {
            _backCampaignRepository = backCampaignRepository;
            _campaignRepository = campaignRepository;
            _campaignService = campaignService;
            _userRepository = userRepository;
            _contentManager = contentManager;
            Services = services;

            _workContextAccessor = workContextAccessor;
        }


        public ActionResult Index()
        {
           var requests = _backCampaignRepository.Table.ToList();
           List<int> campaignsIds = new List<int>();
           foreach (var item in requests)
           {
               campaignsIds.Add(item.CampaignRecord.Id);
           }          
           var noDupesIds = new HashSet<int>(campaignsIds);
           var viewResult = new List<CampaignRelRequestViewModel>();
           foreach (var id in noDupesIds)
           {
               var campaign = _campaignService.GetCampaignById(id);
               if (!campaign.IsActive && !campaign.IsArchived)
               {
                   var listItem = new CampaignRelRequestViewModel()
                   {
                       CampaignTitle = campaign.Title,
                       CampaignId = id,
                       CampaignAlias = campaign.Alias,
                       CntRequests = _campaignService.GetCountOfReservedRequestsOfCampaign(id),
                       Requests = _campaignService.GetReservedRequestsOfCampaign(id).ToList(),
                       Seller = _userRepository.Get(_campaignService.GetCampaignById(id).TeeyootUserId.Value)
                   };

                   viewResult.Add(listItem);
               }
               
           }
           return View(viewResult);       
        }

       
    }
}