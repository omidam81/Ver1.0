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
using Mandrill;
using Mandrill.Model;
using System;

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
            var model = new MessagesIndexViewModel() { };
            model.Campaigns = campaigns;
            return View(model);
        }


        public ActionResult CreateMessage(int campaignId)
        {
            MessageContentViewModel model = new MessageContentViewModel() { };
            model.CampaignId = campaignId;
            model.ProductId = _campaignService.GetCampaignById(campaignId).Products[0].Id;
            return View(model);
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

        public ActionResult SendMessage(MessageContentViewModel m, int campaignId = 0 ,  string culture = "en")
        {
            if (TryUpdateModel(m))
            {
                MailChimpSettingsPart record = _settingsService.GetAllSettings().List().Where(x => x.Culture == culture).FirstOrDefault();
                MailChimpManager mc = new MailChimpManager(record.ApiKey);
                AddUserToMailChimpList(m.Email);
                Thread deleteCampaign = new Thread(delegate(){DeleteSentCampaigns(mc);});
                deleteCampaign.Start();
                Thread crAndSend = new Thread(delegate() { CreateAndSendMessage(m,mc, record); });
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


        public int AddUserToMailChimpList(string email, string firstName = "", string lastName = "", string city = "", string state = "", string country = "", double totalPrice = 0.0, IEnumerable<ProductRecord> products = null, int campaignId = 0)
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

        public void SendWelcomeLetter(string userEmail, string culture)
        {
            var record = _settingsService.GetAllSettings().List().Where( x => x.Culture == culture).FirstOrDefault();
            MailChimpManager mc = new MailChimpManager(record.ApiKey);
            List<string> emails = new List<string>();
            emails.Add(userEmail);
            
            CampaignActionResult response = mc.SendCampaignTest(record.WelcomeCampaignId, emails, "Html");
            
        }

        public ActionResult SendMessageUsingMandrill(MessageContentViewModel model)
        {
            if (TryUpdateModel(model))
            {
                string currentUser = Services.WorkContext.CurrentUser.Email;
                var user = _membershipService.GetUser(currentUser);
                var record = _settingsService.GetAllSettings().List().FirstOrDefault();
                var api = new MandrillApi(record.ApiKey);
                var message = new MandrillMessage() { };
                message.FromEmail = model.From;
                message.Subject = model.Subject;
                message.MergeLanguage = MandrillMessageMergeLanguage.Handlebars;
                List<LinkOrderCampaignProductRecord> ordersList = _orderService.GetProductsOrderedOfCampaign(model.CampaignId).ToList();
                List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                foreach (var item in ordersList)
                {
                    emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
                    FillMessageMergeVars(message, item);
                }
                message.To = emails;
                message.Html = TemplateContent.Code;
                _messageService.AddMessage(user.Id, message.Html, message.FromEmail, DateTime.Now, model.CampaignId);
                //var res = SendTmplMessage(api, message);
                model.Status = "Your message has been sent!";
                return RedirectToAction("Messages");
            }
            return View("CreateMessage", model);
        }



        static void FillMessageMergeVars(MandrillMessage message, LinkOrderCampaignProductRecord record)
        {


            var products = new Dictionary<string, object>
                    {
                        {"quantity", record.Count},
                        {"name",  record.CampaignProductRecord.ProductRecord.Name},
                        {"description",  record.CampaignProductRecord.ProductRecord.Details},
                        {"price",  record.CampaignProductRecord.Price}
                    };

            message.AddRcptMergeVars(record.OrderRecord.Email, "FNAME", record.OrderRecord.FirstName);
            message.AddRcptMergeVars(record.OrderRecord.Email, "LNAME", record.OrderRecord.LastName);
            message.AddRcptMergeVars(record.OrderRecord.Email, "CITY", record.OrderRecord.City);
            message.AddRcptMergeVars(record.OrderRecord.Email, "STATE", record.OrderRecord.State);
            message.AddRcptMergeVars(record.OrderRecord.Email, "COUNTRY", record.OrderRecord.Country);
            if (record.OrderRecord.TotalPriceWithPromo != null)
            {
                message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPriceWithPromo.ToString());
            }
            else
            {
                message.AddRcptMergeVars(record.OrderRecord.Email, "TOTALPRICE", record.OrderRecord.TotalPrice.ToString());
            }
            message.AddRcptMergeVars(record.OrderRecord.Email, "PRODUCTS", products);
        }

         static string SendTmplMessage(MandrillApi mAPI, MandrillMessage message)
        {
             //var message = new MandrillMessage();
            //message.FromEmail = "no-reply@example.com";
            //message.AddTo("kiss-stu@mail.ru");
            ////supports merge var content as string
            //message.AddGlobalMergeVars("invoice_date", DateTime.Now.ToShortDateString());
            ////or as objects (handlebar templates only)
            //message.AddRcptMergeVars("recipient@example.com", "invoice_details", new[]
            //{
            //    new Dictionary<string, object>
            //    {
            //        {"sku", "apples"},
            //        {"qty", 4},
            //        {"price", "0.40"}
            //    },
            //    new Dictionary<string, object>
            //    {
            //        {"sku", "oranges"},
            //        {"qty", 6},
            //        {"price", "0.30"}

            //    }
            //});

            //return await mAPI.Messages.SendTemplateAsync(message, "customer-invoice");
            //int i = 0;
            //var message = new MandrillMessage
            //{
            //    FromEmail = "mandrill.net@sollutium.com",
            //    Subject = "test",
            //    //Tags = new List<string>() { "test-send-template", "mandrill-net", "handlebars" },
            //    MergeLanguage = MandrillMessageMergeLanguage.Handlebars, //NOTE: обязательно, эта штука позволяет интерпретировать переменные с фигурными скобками
                
            //    To = emails,
            //    Html = TemplateContent.Code,
            //    Text = "this is a text inside message",
            //};


            //var data1 = new[]
            //    {
            //        new Dictionary<string, object>
            //        {
            //            {"sku", "KISS-STU ORA53"},
            //            {"name", "Oranges"},
            //            {"description", "Sunkist Oranges"},
            //            {"price", 0.20},
            //            {"qty", 1},
            //            {"ordPrice", 0.20},

            //        },
            //        new Dictionary<string, object>
            //        {
            //             {"sku", "KISS-STU APL54"},
            //            {"name", "apples"},
            //            {"description", "Red Delicious Apples"},
            //            {"price", 0.22},
            //            {"qty", 9},
            //            {"ordPrice", 1.98},
            //        }
            //    };

            //var data2 = new[]
            //    {
            //         new Dictionary<string, object>
            //        {
            //            {"sku", "IK@TRUNK APL43"},
            //            {"name", "apples"},
            //            {"description", "Granny Smith Apples"},
            //            {"price", 0.20},
            //            {"qty", 8},
            //            {"ordPrice", 1.60},

            //        },
            //        new Dictionary<string, object>
            //        {
            //            {"sku", "IK@TRUNK ORA44"},
            //            {"name", "Oranges"},
            //            {"description", "Blood Oranges"},
            //            {"price", 0.30},
            //            {"qty", 3},
            //            {"ordPrice", 0.93},

            //        }
            //    };

            ////TODO: раскидать по методам заполнения параметров
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "FNAME", "Kiss");
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "LNAME", "Stu");
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "CITY", "Krakow");
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "STATE", "Malopolskie");
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "COUNTRY", "Poland");
            //message.AddRcptMergeVars("kiss-stu@mail.ru", "PRODUCTS", data1);

            //message.AddRcptMergeVars("ik@trunk.net.ua", "FNAME", "IK");
            //message.AddRcptMergeVars("ik@trunk.net.ua", "LNAME", "TRUNK");
            //message.AddRcptMergeVars("ik@trunk.net.ua", "CITY", "KIEV");
            //message.AddRcptMergeVars("ik@trunk.net.ua", "STATE", "KYIVSKA OBL");
            //message.AddRcptMergeVars("ik@trunk.net.ua", "COUNTRY", "UKRAINE");
            //message.AddRcptMergeVars("ik@trunk.net.ua", "PRODUCTS", data2);
            
            //var result = await mAPI.Messages.SendTemplateAsync(message, TestTemplateName);
            //var result = await mAPI.Messages.SendAsync(message);
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }


        struct TemplateContent
        {
            public const string Code = @"<html>
<head>
	<title>a test TeeYoot</title>
</head>
<body>
	
	<p>Dear {{FNAME}}, your Last name is {{LNAME}}</p>
   <p>Thank you for your purchase in city {{CITY}} and state {{STATE}} in country {{COUNTRY}} the product {{PRODUCTS}} from ABC Widget Company. <br>
We appreciate your business and have included a copy of your invoice below. <br>

<!-- BEGIN PRODUCT LOOP // -->
   {{#each products}}
   <tr class=""item"">
        <td valign=""top"" class=""textContent"">
            <h4 class=""itemName"">{{name}}</h4>
            <span class=""contentSecondary"">Qty: {{qty}} x ${{price}}/each</span><br />
            <span class=""contentSecondary sku""><em>{{sku}}</em></span><br />
            <span class=""contentSecondary itemDescription"">{{description}}</span>
        </td>
        <td valign=""top"" class=""textContent alignRight priceWidth"">
            ${{ordPrice}}
        </td>
    </tr>
    {{/each}}
<!-- // END PRODUCT LOOP -->

<br>
The totlal price will be {{TOTALPRICE}}
<br>
Please let us know if you have further questions.
     -- ABC Widget Co.</p>

     <div mc:edit=""footer"">footer</div>
</body>
</html>";
            public const string Text = @"Dear {{FNAME}},
   Thank you for your purchase in city {{CITY}} and state {{STATE}} in country {{COUNTRY}} the product {{PRODUCTS}} from ABC Widget Company.. 
We appreciate your business and have included a copy of your invoice below.

Please let us know if you have further questions.

     -- ABC Widget Co.";

            public const string HandleBarCode = @"<html>
<head>
	<title>a test</title>
</head>
<body>
	
	<p>Dear{{fname}},</p>
   <p>Thank you for your purchase on {{orderdate}} from ABC Widget Company. <br>
We appreciate your business and have included a copy of your invoice below. <br>
   <!-- BEGIN PRODUCT LOOP // -->
   {{#each products}}
   <tr class=""item"">
        <td valign=""top"" class=""textContent"">
            <h4 class=""itemName"">{{name}}</h4>
            <span class=""contentSecondary"">Qty: {{qty}} x ${{price}}/each</span><br />
            <span class=""contentSecondary sku""><em>{{sku}}</em></span><br />
            <span class=""contentSecondary itemDescription"">{{description}}</span>
        </td>
        <td valign=""top"" class=""textContent alignRight priceWidth"">
            ${{ordPrice}}
        </td>
    </tr>
    {{/each}}
<!-- // END PRODUCT LOOP -->
Please let us know if you have further questions.


     -- ABC Widget Co.</p>

     <div mc:edit=""footer"">footer</div>
</body>
</html>";
        }
    }



}