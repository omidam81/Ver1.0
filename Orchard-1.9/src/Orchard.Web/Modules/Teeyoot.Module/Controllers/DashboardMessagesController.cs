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


namespace Teeyoot.Module.Controllers
{
    public partial class DashboardController : Controller
    {
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
        public ActionResult SendMessage(MessageContentRecord m)
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
                return View("Index", ViewBag);
            }
            return View("CreateMessage");
        }
    }
}