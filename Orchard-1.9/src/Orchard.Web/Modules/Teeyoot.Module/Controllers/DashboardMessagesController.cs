using Orchard.Themes;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using Teeyoot.Module.Models;
using MailChimp;
using MailChimp.Campaigns;
using MailChimp.Helper;
using MailChimp.Lists;
using Orchard.Data;
using MailChimp.Templates;


namespace Teeyoot.Module.Controllers
{
    public partial class DashboardController : Controller
    {

        private readonly IRepository<MailChimpSettingsPartRecord> mailChimpRepository;

        public DashboardController(IRepository<MailChimpSettingsPartRecord> mailChimpRepository)
        {
            this.mailChimpRepository = mailChimpRepository;
        }


        // GET: Message
        public ActionResult Messages()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateMessage()
        {
            return View();
        }
        public ActionResult SendMessage(MessageContentViewModel m)
        {
            if (TryUpdateModel(m))
            {

                MailChimpManager mc = new MailChimpManager("efaebe4525defd02eec94579dfc7f4ce-us11");
                CampaignCreateOptions options = new CampaignCreateOptions() { };
                options.FromEmail = m.From;
                options.FromName = "Teeyoot seller";
                options.Subject = m.Subject;
                options.ListId = "f90603a2b0";
                CampaignCreateContent content = new CampaignCreateContent() { };
                content.Text = "vcx";
                Campaign myCampaign = mc.CreateCampaign("regular", options, content);
                CampaignContent newCont = new CampaignContent() { };
                newCont.Text = m.Content;
                CampaignUpdateResult upd = mc.UpdateCampaign(myCampaign.Id, "content", newCont);
                List<string> emails = new List<string>();
                emails.Add(m.Email);
                CampaignActionResult response = mc.SendCampaignTest(myCampaign.Id, emails, "Text");
                if (response.Complete)
                {
                    ViewBag.status = "Your message has been sended!";

                }
                return View("Messages", ViewBag);
            }
            return View("CreateMessage");
        }

        public int AddUserToMailChimpList(string email)
        {
            MailChimpSettingsPartRecord record = mailChimpRepository.Fetch(x => x.Id == 1).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            EmailParameter emailParam = new EmailParameter() { };
            emailParam.Email = email;
            EmailParameter subscr = mc.Subscribe(record.MailChimpListId, emailParam, null, "html", false);
            if (subscr.LEId == null)
            {
                return -1;
            }
            return 1;
        }

        public string SendWelcomeLetter(string userEmail)
        {
            MailChimpSettingsPartRecord record = mailChimpRepository.Fetch(x => x.TemplateName == "Welcome").FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            List<string> emails = new List<string>();
            emails.Add(userEmail);
            CampaignActionResult response = mc.SendCampaignTest(record.MailChimpCampaignId, emails, "Text");
            if (response.Complete)
            {
                ViewBag.status = "Your message has been sended!";

            }
            return ViewBag.status;
        }
    }
}