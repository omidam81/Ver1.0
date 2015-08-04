using MailChimp;
using MailChimp.Campaigns;
using MailChimp.Helper;
using MailChimp.Lists;
using Mandrill;
using Mandrill.Model;
using Orchard.UI.Notify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Messaging.Models;
using Teeyoot.Module.Dashboard.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        // GET: Message
        public ActionResult Messages()
        {
            string currentUser = Services.WorkContext.CurrentUser.Email;
            var user = _membershipService.GetUser(currentUser);
            var campaigns = _campaignService.GetCampaignsOfUser(user.Id).ToList();
            List<MessagesIndexViewModel> listModel = new List<MessagesIndexViewModel>();
            foreach (var item in campaigns)
            {                
                var tempModel = new MessagesIndexViewModel() { };
                tempModel.ThisWeekSend = _messageService.GetAllMessagesForCampaign(item.Id).Where(s => (s.SendDate < DateTime.UtcNow) && (s.SendDate > DateTime.UtcNow.AddDays(-7))).Count();
                if (_messageService.GetLatestMessageDateForCampaign(item.Id).Day > DateTime.UtcNow.Day) 
                {
                    tempModel.LastSend = "Never";
                }
                else
                {
                    if (DateTime.UtcNow.Day - _messageService.GetLatestMessageDateForCampaign(item.Id).Day == 0)
                    {
                        tempModel.LastSend = "Today";
                    }
                    else
                    {
                        tempModel.LastSend = (DateTime.UtcNow.Day - _messageService.GetLatestMessageDateForCampaign(item.Id).Day).ToString() + " days ago";
                    }
                }

                tempModel.Campaign = item;
                listModel.Add(tempModel);

            }
            IEnumerable<MessagesIndexViewModel> model = listModel;
            return View(model);
        }


        public ActionResult CreateMessage(int campaignId)
        {
            MessageContentViewModel model = new MessageContentViewModel() { };
            model.CampaignId = campaignId;
            var campaign = _campaignService.GetCampaignById(campaignId);
            model.ProductId = campaign.Products[0].Id;
            model.CampaignTitle = campaign.Title;
            return View(model);
        }


        public void CreateAndSendMessage(MessageContentViewModel m, MailChimpManager mc, MailChimpSettingsPart record, string culture = "en", int campaignId = 0)
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
            //emails.Add(m.Email);
            CampaignActionResult response = mc.SendCampaign(myCampaign.Id);
            if (response.Complete)
            {

            }
        }

        public ActionResult SendMessage(MessageContentViewModel m, int campaignId = 0, string culture = "en")
        {
            if (TryUpdateModel(m))
            {
                MailChimpSettingsPart record = _settingsService.GetAllSettings().List().Where(x => x.Culture == culture).FirstOrDefault();
                MailChimpManager mc = new MailChimpManager(record.ApiKey);
                //AddUserToMailChimpList(m.Email);
                Thread deleteCampaign = new Thread(delegate() { DeleteSentCampaigns(mc); });
                deleteCampaign.Start();
                Thread crAndSend = new Thread(delegate() { CreateAndSendMessage(m, mc, record); });
                crAndSend.Start();
                ViewBag.Status = "Your message has been sent!";
                return View("Messages", ViewBag);
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
                if (campaign.Title != "Welcome")
                    mc.DeleteCampaign(campaign.Id);
            }

        }


        public int SendMessageToCampaignBuyers(int campaignId, string culture)
        {

            var record = _settingsService.GetAllSettings().List().Where(x => x.Culture == culture).FirstOrDefault();
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


        public int AddUserToMailChimpList(string email, string firstName = "", string lastName = "", string city = "", string state = "", string country = "", double totalPrice = 0.0, IEnumerable<ProductRecord> products = null, int campaignId = 0)
        {
            var record = _settingsService.GetAllSettings().List().FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            EmailParameter emailParam = new EmailParameter() { };
            MergeVar mergeVar = new MergeVar() { };
            mergeVar.Add("FNAME", firstName);
            mergeVar.Add("LNAME", lastName);
            mergeVar.Add("CAMPAIGNID", "#" + campaignId + "#");
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

        public void SendWelcomeLetter(string userEmail, string culture)
        {
            var record = _settingsService.GetAllSettings().List().Where(x => x.Culture == culture).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            List<string> emails = new List<string>();
            emails.Add(userEmail);

            CampaignActionResult response = mc.SendCampaignTest(record.WelcomeCampaignId, emails, "Html");

        }

        public ActionResult SendSellerMessageForApproving(MessageContentViewModel model)
        {
            if (TryUpdateModel(model))
            {
                string currentUser = Services.WorkContext.CurrentUser.Email;
                var user = _membershipService.GetUser(currentUser);
                //var record = _settingsService.GetAllSettings().List().FirstOrDefault();
                //var api = new MandrillApi(record.ApiKey);
                //var message = new MandrillMessage() { };
                //message.FromEmail = model.From;
                //message.Subject = model.Subject;
                //message.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                //List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(model.CampaignId).ToList();
                //var campaign = _campaignService.GetCampaignById(model.CampaignId);
                //List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                //foreach (var item in ordersList)
                //{
                //    emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
                //    FillMessageMergeVars(message, item);
                //}
                //message.To = emails;
                //string messageText = TemplateContent.Template.Replace("---MessageContent---",model.Content);
                //messageText = messageText.Replace("---SellerEmail---", user.Email);
                //messageText = messageText.Replace("---CampaignTitle---", model.CampaignTitle);
                //string previewUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/')+ "/Media/campaigns/"+model.CampaignId+"/"+campaign.Products[0].Id+"/normal/front.png";
                //messageText = messageText.Replace("---CampaignPreviewUrl---", previewUrl);
                //message.Html = messageText;
                _messageService.AddMessage(user.Id, model.Content, model.From, DateTime.UtcNow, model.CampaignId, model.Subject, false);
                //var res = SendTmplMessage(api, message);
                _notifier.Information(T("Your message has been sent for approving!"));
                return RedirectToAction("Messages");
            }
            return View("CreateMessage", model);
        }

        

        


 
        
    }



}