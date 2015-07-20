using MailChimp;
using MailChimp.Campaigns;
using MailChimp.Helper;
using MailChimp.Lists;
using MailChimp.Templates;
using Orchard.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Teeyoot.Messaging.Models;
using Teeyoot.Messaging.Services;
using Teeyoot.Module.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using System.Threading;

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


        public void CreateAndSendMessage(MessageContentViewModel m,  MailChimpManager mc, MailChimpSettingsPart record, string culture = "en", int campaignId = 0)
        {
            CampaignCreateOptions options = new CampaignCreateOptions() { };
            options.FromEmail = m.From;
            options.FromName = "Teeyoot seller";
            options.Subject = m.Subject;
            options.ListId = record.MailChimpListId;
            options.TemplateID = record.AllBuyersTemplateId;
            CampaignCreateContent content = new CampaignCreateContent()
            {
                Sections = new Dictionary<string, string>()
            };
            content.Sections.Add("body_section", m.Content);
            content.Sections.Add("seller_email", m.From);
            Campaign myCampaign = mc.CreateCampaign("regular", options, content);
            List<string> emails = new List<string>();
            emails.Add(m.Email);
            CampaignActionResult response = mc.SendCampaign(myCampaign.Id);
            if (response.Complete)
            {

            }
        }

        public ActionResult SendMessage(MessageContentViewModel m, string culture = "en", int campaignId = 0)
        {
            if (TryUpdateModel(m))
            {
                MailChimpSettingsPart record = _settingsService.GetAllSettings().List().Where(x => x.Culture == culture).FirstOrDefault();
                MailChimpManager mc = new MailChimpManager(record.ApiKey);
                Thread deleteCampaign = new Thread(delegate(){DeleteSentCampaigns(mc);});
                deleteCampaign.Start();
                Thread crAndSend = new Thread(delegate() { CreateAndSendMessage(m,mc, record); });
                crAndSend.Start();
                m.Status = "Your message has been sent!";
                return View("Messages", m);
            }
            return View("CreateMessage");
        }


        public void DeleteSentCampaigns(MailChimpManager mc)
        {
            CampaignFilter campaignFilter = new CampaignFilter() { };
            campaignFilter.Status = "sent";
            CampaignListResult sentCampaigns = mc.GetCampaigns(campaignFilter);
            foreach (var campaign in sentCampaigns.Data)
            {
                if(campaign.Title != "Welcome")
                mc.DeleteCampaign(campaign.Id);
            }
            
        }
        
        
        public int SendMessageToCampaignBuyers(int campaignId , string culture)
        {

            var record = _settingsService.GetAllSettings().List().Where( x => x.Culture == culture).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);

            List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(campaignId).ToList();
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


        public int AddUserToMailChimpList(string email, string firstName, string lastName, string city, string state, string country, double totalPrice, IEnumerable<ProductRecord> products = null, int campaignId = 0)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            EmailParameter emailParam = new EmailParameter(){};
            MergeVar mergeVar = new MergeVar() { };
            mergeVar.Add("FNAME", firstName);
            mergeVar.Add("LNAME",lastName);
            mergeVar.Add("CAMPAIGNID", "#"+campaignId+"#");
            mergeVar.Add("CITY", city);
            mergeVar.Add("STATE", state);
            mergeVar.Add("COUNTRY", country);
            mergeVar.Add("TOTALPRICE", totalPrice);
            mergeVar.Add("PRODUCTS", products);
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