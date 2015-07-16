﻿using Orchard.UI.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Messaging.Services;
using Teeyoot.Messaging.ViewModels;

namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminMessageController : Controller
    {
        private readonly IMailChimpSettingsService _settingsService;

        public AdminMessageController(IMailChimpSettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        
        public ActionResult Index()
        {
            var settings = _settingsService.GetAllSettings().List().Select(s => new MailChimpListViewModel 
                                            {
                                                ApiKey = s.ApiKey,
                                                MailChimpCampaignId = s.MailChimpCampaignId,
                                                MailChimpListId = s.MailChimpListId,
                                                TemplateId = s.TemplateId,
                                                TemplateName = s.TemplateName
                                            }); 
            return View(settings);
        }

        public ActionResult AddSetting()
        {
            return View();
        }
    }
}