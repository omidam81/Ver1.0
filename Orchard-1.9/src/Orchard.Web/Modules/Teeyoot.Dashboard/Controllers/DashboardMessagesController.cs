using MailChimp;
using MailChimp.Campaigns;
using MailChimp.Helper;
using MailChimp.Lists;
using MailChimp.Templates;
using Orchard.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Teeyoot.Module.Dashboard.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Controllers
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
                options.TemplateID = 75037;
                TemplateFilters templateFilter = new TemplateFilters() { };
                TemplateInformationResult template = mc.GetTemplateInformation(75037);
                TemplateUpdateValue updTemplate = new TemplateUpdateValue(){};
                CampaignCreateContent content = new CampaignCreateContent() { };
                content.Text = "vcx";
                Campaign myCampaign = mc.CreateCampaign("regular", options, content);
                CampaignContent newCont = new CampaignContent() { };
                newCont.Text = m.Content;
                CampaignUpdateResult upd = mc.UpdateCampaign(myCampaign.Id, "content", newCont);
                List<string> emails = new List<string>();
                emails.Add(m.Email);
                CampaignActionResult response = mc.SendCampaignTest(myCampaign.Id, emails, "Html");
                if (response.Complete)
                {
                    ViewBag.status = "Your message has been sended!";

                }
                return View("Messages", ViewBag);
            }
            return View("CreateMessage");
        }


        public int SendMessageToCampaignBuyers(int campaignId , string culture)
        {           
                
            var record = _settingsService.GetAllSettings().List().Where( x => x.Culture == culture).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            List<LinkOrderCampaignProductRecord> ordersList = _linkOrderCampaignProductRepository.Table.Fetch(x => x.CampaignProductRecord.CampaignRecord_Id == campaignId).ToList();
            List<string> emails = new List<string>();
            foreach (var item in ordersList)
            {
                emails.Add(item.OrderRecord.Email);
            }
            CampaignActionResult response = mc.SendCampaignTest(record.AllBuyersCampaignId, emails, "Text");
                if (!response.Complete)
                {
                    return -1;
                }
                return 1;
            
        }


        public int AddUserToMailChimpList(string email, string firstName, string lastName)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            EmailParameter emailParam = new EmailParameter(){};
            MergeVar mergeVar = new MergeVar() { };
            mergeVar.Add("FNAME", firstName);
            mergeVar.Add("LNAME", lastName);
            emailParam.Email = email;
            EmailParameter subscr = mc.Subscribe(record.MailChimpListId, emailParam, mergeVar, "html", false, true);
            if (subscr.LEId != null)
            {
                return 1;
            }
            return -1;
        }

        public int SendWelcomeLetter(string userEmail, string culture)
        {
            var record = _settingsService.GetAllSettings().List().Where( x => x.Culture == culture).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            List<string> emails = new List<string>();
            emails.Add(userEmail);
            CampaignActionResult response = mc.SendCampaignTest(record.WelcomeCampaignId, emails, "Html");
            if (response.Complete != true)
            {
                return -1;
            }
            return 1;
        }

    }
}