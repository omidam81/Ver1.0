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
                var campaign = _campaignService.GetCampaignById(model.CampaignId);
                List<MandrillMailAddress> emails = new List<MandrillMailAddress>();
                foreach (var item in ordersList)
                {
                    emails.Add(new MandrillMailAddress(item.OrderRecord.Email, "user"));
                    FillMessageMergeVars(message, item);
                }
                message.To = emails;
                string messageText = TemplateContent.Template.Replace("---MessageContent---",model.Content);
                messageText = messageText.Replace("---SellerEmail---", user.Email);
                messageText = messageText.Replace("---CampaignTitle---", model.CampaignTitle);
                string previewUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/')+ "/Media/campaigns/"+model.CampaignId+"/"+campaign.Products[0].Id+"/normal/front.png";
                messageText = messageText.Replace("---CampaignPreviewUrl---", previewUrl);
                message.Html = messageText;
                _messageService.AddMessage(user.Id, model.Content, message.FromEmail, DateTime.UtcNow, model.CampaignId, model.Subject, false);
                var res = SendTmplMessage(api, message);
                _notifier.Information(T("Your message has been sent!"));
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
            if (record.OrderRecord.TotalPriceWithPromo > 0.0 )
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
            var result = mAPI.Messages.Send(message);
            return result.ToString();
        }


        struct TemplateContent
        {
            public const string Template = @"
<html>
    <head>
    	<!-- NAME: 1 COLUMN -->
        <meta name=""""viewport"""" content=""""width=device-width, initial-scale=1.0"""">
        <title>*|MC:SUBJECT|*</title>
<style type=""""text/css"""">
		body,#bodyTable,#bodyCell{
			height:100% !important;
			margin:0;
			padding:0;
			width:100% !important;
		}
		table{
			border-collapse:collapse;
		}
		img,a img{
			border:0;
			outline:none;
			text-decoration:none;
		}
		h1,h2,h3,h4,h5,h6{
			margin:0;
			padding:0;
		}
		p{
			margin:1em 0;
			padding:0;
		}
		a{
			word-wrap:break-word;
		}
		.ReadMsgBody{
			width:100%;
		}
		.ExternalClass{
			width:100%;
		}
		.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{
			line-height:100%;
		}
		table,td{
			mso-table-lspace:0pt;
			mso-table-rspace:0pt;
		}
		#outlook a{
			padding:0;
		}
		img{
			-ms-interpolation-mode:bicubic;
		}
		body,table,td,p,a,li,blockquote{
			-ms-text-size-adjust:100%;
			-webkit-text-size-adjust:100%;
		}
		#bodyCell{
			padding:20px;
		}
		.mcnImage{
			vertical-align:bottom;
		}
		.mcnTextContent img{
			height:auto !important;
		}

		body,#bodyTable{
			/*@editable*/background-color:#F2F2F2;
		}

		#bodyCell{
			/*@editable*/border-top:0;
		}

		#templateContainer{
			/*@editable*/border:0;
		}

		h1{
			/*@editable*/color:#606060 !important;
			display:block;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:40px;
			/*@editable*/font-style:normal;
			/*@editable*/font-weight:bold;
			/*@editable*/line-height:125%;
			/*@editable*/letter-spacing:-1px;
			margin:0;
			/*@editable*/text-align:left;
		}

		h2{
			/*@editable*/color:#404040 !important;
			display:block;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:26px;
			/*@editable*/font-style:normal;
			/*@editable*/font-weight:bold;
			/*@editable*/line-height:125%;
			/*@editable*/letter-spacing:-.75px;
			margin:0;
			/*@editable*/text-align:left;
		}

		h3{
			/*@editable*/color:#606060 !important;
			display:block;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:18px;
			/*@editable*/font-style:normal;
			/*@editable*/font-weight:bold;
			/*@editable*/line-height:125%;
			/*@editable*/letter-spacing:-.5px;
			margin:0;
			/*@editable*/text-align:left;
		}

		h4{
			/*@editable*/color:#808080 !important;
			display:block;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:16px;
			/*@editable*/font-style:normal;
			/*@editable*/font-weight:bold;
			/*@editable*/line-height:125%;
			/*@editable*/letter-spacing:normal;
			margin:0;
			/*@editable*/text-align:left;
		}


		#templatePreheader{
			/*@editable*/background-color:#FFFFFF;
			/*@editable*/border-top:0;
			/*@editable*/border-bottom:0;
		}

		.preheaderContainer .mcnTextContent,.preheaderContainer .mcnTextContent p{
			/*@editable*/color:#606060;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:11px;
			/*@editable*/line-height:125%;
			/*@editable*/text-align:left;
		}

		.preheaderContainer .mcnTextContent a{
			/*@editable*/color:#606060;
			/*@editable*/font-weight:normal;
			/*@editable*/text-decoration:underline;
		}

		#templateHeader{
			/*@editable*/background-color:#FFFFFF;
			/*@editable*/border-top:0;
			/*@editable*/border-bottom:0;
		}

		.headerContainer .mcnTextContent,.headerContainer .mcnTextContent p{
			/*@editable*/color:#606060;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:15px;
			/*@editable*/line-height:150%;
			/*@editable*/text-align:left;
		}

		.headerContainer .mcnTextContent a{
			/*@editable*/color:#6DC6DD;
			/*@editable*/font-weight:normal;
			/*@editable*/text-decoration:underline;
		}

		#templateBody{
			/*@editable*/background-color:#FFFFFF;
			/*@editable*/border-top:0;
			/*@editable*/border-bottom:0;
		}

		.bodyContainer .mcnTextContent,.bodyContainer .mcnTextContent p{
			/*@editable*/color:#606060;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:15px;
			/*@editable*/line-height:150%;
			/*@editable*/text-align:left;
		}

		.bodyContainer .mcnTextContent a{
			/*@editable*/color:#6DC6DD;
			/*@editable*/font-weight:normal;
			/*@editable*/text-decoration:underline;
		}

		#templateFooter{
			/*@editable*/background-color:#FFFFFF;
			/*@editable*/border-top:0;
			/*@editable*/border-bottom:0;
		}

		.footerContainer .mcnTextContent,.footerContainer .mcnTextContent p{
			/*@editable*/color:#606060;
			/*@editable*/font-family:Helvetica;
			/*@editable*/font-size:11px;
			/*@editable*/line-height:125%;
			/*@editable*/text-align:left;
		}

		.footerContainer .mcnTextContent a{
			/*@editable*/color:#606060;
			/*@editable*/font-weight:normal;
			/*@editable*/text-decoration:underline;
		}
	@media only screen and (max-width: 480px){
		body,table,td,p,a,li,blockquote{
			-webkit-text-size-adjust:none !important;
		}

}	@media only screen and (max-width: 480px){
		body{
			width:100% !important;
			min-width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		td[id=bodyCell]{
			padding:10px !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnTextContentContainer]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnBoxedTextContentContainer]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcpreview-image-uploader]{
			width:100% !important;
			display:none !important;
		}

}	@media only screen and (max-width: 480px){
		img[class=mcnImage]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnImageGroupContentContainer]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageGroupContent]{
			padding:9px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageGroupBlockInner]{
			padding-bottom:0 !important;
			padding-top:0 !important;
		}

}	@media only screen and (max-width: 480px){
		tbody[class=mcnImageGroupBlockOuter]{
			padding-bottom:9px !important;
			padding-top:9px !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnCaptionTopContent],table[class=mcnCaptionBottomContent]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnCaptionLeftTextContentContainer],table[class=mcnCaptionRightTextContentContainer],table[class=mcnCaptionLeftImageContentContainer],table[class=mcnCaptionRightImageContentContainer],table[class=mcnImageCardLeftTextContentContainer],table[class=mcnImageCardRightTextContentContainer]{
			width:100% !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardLeftImageContent],td[class=mcnImageCardRightImageContent]{
			padding-right:18px !important;
			padding-left:18px !important;
			padding-bottom:0 !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardBottomImageContent]{
			padding-bottom:9px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardTopImageContent]{
			padding-top:18px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardLeftImageContent],td[class=mcnImageCardRightImageContent]{
			padding-right:18px !important;
			padding-left:18px !important;
			padding-bottom:0 !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardBottomImageContent]{
			padding-bottom:9px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnImageCardTopImageContent]{
			padding-top:18px !important;
		}

}	@media only screen and (max-width: 480px){
		table[class=mcnCaptionLeftContentOuter] td[class=mcnTextContent],table[class=mcnCaptionRightContentOuter] td[class=mcnTextContent]{
			padding-top:9px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnCaptionBlockInner] table[class=mcnCaptionTopContent]:last-child td[class=mcnTextContent]{
			padding-top:18px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnBoxedTextContentColumn]{
			padding-left:18px !important;
			padding-right:18px !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=mcnTextContent]{
			padding-right:18px !important;
			padding-left:18px !important;
		}

}	@media only screen and (max-width: 480px){

		table[id=templateContainer],table[id=templatePreheader],table[id=templateHeader],table[id=templateBody],table[id=templateFooter]{
max-width:600px !important;
			/*@editable*/width:100% !important;
		}

}	@media only screen and (max-width: 480px){

		h1{
			/*@editable*/font-size:24px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		h2{
			/*@editable*/font-size:20px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		h3{
			/*@editable*/font-size:18px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		h4{
			/*@editable*/font-size:16px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		table[class=mcnBoxedTextContentContainer] td[class=mcnTextContent],td[class=mcnBoxedTextContentContainer] td[class=mcnTextContent] p{
			/*@editable*/font-size:18px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		table[id=templatePreheader]{
			/*@editable*/display:block !important;
		}

}	@media only screen and (max-width: 480px){

		td[class=preheaderContainer] td[class=mcnTextContent],td[class=preheaderContainer] td[class=mcnTextContent] p{
			/*@editable*/font-size:14px !important;
			/*@editable*/line-height:115% !important;
		}

}	@media only screen and (max-width: 480px){

		td[class=headerContainer] td[class=mcnTextContent],td[class=headerContainer] td[class=mcnTextContent] p{
			/*@editable*/font-size:18px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){

		td[class=bodyContainer] td[class=mcnTextContent],td[class=bodyContainer] td[class=mcnTextContent] p{
			/*@editable*/font-size:18px !important;
			/*@editable*/line-height:125% !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=footerContainer] td[class=mcnTextContent],td[class=footerContainer] td[class=mcnTextContent] p{
			/*@editable*/font-size:14px !important;
			/*@editable*/line-height:115% !important;
		}

}	@media only screen and (max-width: 480px){
		td[class=footerContainer] a[class=utilityLink]{
			display:block !important;
		}

}</style></head>
    <body leftmargin=""""0"""" marginwidth=""0"" topmargin=""0"" marginheight=""0"" offset=""0"">
        <center>
            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" height=""100%"" width=""100%"" id=""bodyTable"">
                <tr>
                    <td align=""center"" valign=""top"" id=""bodyCell"">
                        <!-- BEGIN TEMPLATE // -->
                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" id=""templateContainer"">
                            <tr>
                                <td align=""center"" valign=""top"">
                                    <!-- BEGIN PREHEADER // -->
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" id=""templatePreheader"">
                                        <tr>
                                        	<td valign=""top"" class=""preheaderContainer"" style=""padding-top:9px;""></td>
                                        </tr>
                                    </table>
                                    <!-- // END PREHEADER -->
                                </td>
                            </tr>
                            <tr>
                                <td align=""center"" valign=""top"">
                                    <!-- BEGIN HEADER // -->
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" id=""templateHeader"">
                                        <tr>
                                            <td valign=""top"" class=""headerContainer""><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""mcnImageBlock"">
    <tbody class=""mcnImageBlockOuter"">
            <tr>
                <td valign=""top"" style=""padding:9px"" class=""mcnImageBlockInner"">
                    <table align=""left"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""mcnImageContentContainer"">
                        <tbody><tr>
                            <td class=""mcnImageContent"" valign=""top"" style=""padding-right: 9px; padding-left: 9px; padding-top: 0; padding-bottom: 0;"">
                                
                                    
                                        <img align=""left"" alt="""" src=""https://gallery.mailchimp.com/9e4bf6e703cbddc2d472096f3/images/9e35db78-545f-439a-959a-d14eea89dcc5.png"" width=""564"" style=""max-width:3333px; padding-bottom: 0; display: inline !important; vertical-align: bottom;"" class=""mcnImage"">
                                    
                                
                            </td>
                        </tr>
                    </tbody></table>
                </td>
            </tr>
    </tbody>
</table></td>
                                        </tr>
                                    </table>
                                    <!-- // END HEADER -->
                                </td>
                            </tr>
                            <tr>
                                <td align=""center"" valign=""top"">
                                    <!-- BEGIN BODY // -->
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" id=""templateBody"">
                                        <tr>
                                            <td valign=""top"" class=""bodyContainer""><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""mcnTextBlock"">
    <tbody class=""mcnTextBlockOuter"">
        <tr>
            <td valign=""top"" class=""mcnTextBlockInner"">
                
                <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""mcnTextContentContainer"">
                    <tbody><tr>
                        
                        <td valign=""top"" class=""mcnTextContent"" style=""padding-top:9px; padding-right: 18px; padding-bottom: 9px; padding-left: 18px;"">
                        
                            <span style=""color:#000000""><span style=""font-family:arial,helvetica neue,helvetica,sans-serif"">A message from the seller of:</span></span><br>
<br>
<span style=""color:#3399ff""><span style=""font-family:arial,helvetica neue,helvetica,sans-serif""><span style=""font-size:32px"">---CampaignTitle---</span></span></span>
<img src = ""---CampaignPreviewUrl---"">
<br>
<br>
<br>
&nbsp;
<p><font color=""#000000"" face=""arial, helvetica neue, helvetica, sans-serif"">---MessageContent---
       <br>
<br>
<span style=""font-size:17px""><strong>Seller Email:</strong></span> </font><span style=""color:#3399ff""><font face=""arial, helvetica neue, helvetica, sans-serif""><span style=""font-size:15px"">---SellerEmail---</span></font></span><br>
<br>
<font color=""#000000"" face=""arial, helvetica neue, helvetica, sans-serif"">To unsubscribe from emails this seller, please click here: </font><span style=""color:#3399ff""><font face=""arial, helvetica neue, helvetica, sans-serif"">Unsubscribe</font></span></p>

                        </td>
                    </tr>
                </tbody></table>
                
            </td>
        </tr>
    </tbody>
</table></td>
                                        </tr>
                                    </table>
                                    <!-- // END BODY -->
                                </td>
                            </tr>
                            <tr>
                                <td align=""center"" valign=""top"">
                                    <!-- BEGIN FOOTER // -->
                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" id=""templateFooter"">
                                        <tr>
                                            <td valign=""top"" class=""footerContainer"" style=""padding-bottom:9px;""><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""mcnTextBlock"">
    <tbody class=""mcnTextBlockOuter"">
        <tr>
            <td valign=""top"" class=""mcnTextBlockInner"">
                
                <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" class=""mcnTextContentContainer"">
                    <tbody><tr>
                        
                        <td valign=""top"" class=""mcnTextContent"" style=""padding-top:9px; padding-right: 18px; padding-bottom: 9px; padding-left: 18px;"">
                        
                            <p style=""line-height: normal;text-align: -webkit-center;margin: 0px 0px 1em;color: #8A9499;font-family: Helvetica, Arial, sans-serif;font-size: 11px;"">From your friends at&nbsp;<a href=""http://email.teespring.com/wf/click?upn=73rCXRPHaXFyWCZK9SXbxd9fhYeu3sfZ9G8JUnjJDFM-3D_4rgnTcurgLEonc3GE3bj0fnAewlVIvhouLMzi2a7m-2B6796UWSvlGEPGOcSXCNHfESV4NpUiYqXxDIsaizX02YuVrIvtdOBgXAVRwssIdK1EhDDDXg0mKqxk4eu9eqj-2FcJ4lZUOpGVfIL9xwpoZ9c7saFWS4b9v2wFiKveGRnIYmt2AfxY7vbGH-2F7z7RdVcpuVvcVJEpKiujpQoa7zY62qcYyPoszYeJKZpl-2BH00u-2FvLzThb9x0yjBH7fHO1V9tSIw3Sf8G5ARCQGK2YDJscCGTJcVeg7KJuMAf4HkHp585gDnOFAmzWyhPmsVzfO0eF4tEqTf-2FZPcs1QLP6-2BMTqD30-2BhVjAKBItg0352EEwWhTF7t629Qt5u3nZO2fso0waWIRMpCYO4jipL12IKDn15ZlXmkwokpzVyTmlpI4OJTMfq2oWBNzkzKdyYbLh7pMNt"" style=""color: #5C6366;text-decoration: none;"" target=""_blank"" title=""Visit Teespring.com""><span style=""color:#000000"">Teey</span></a><span style=""color:#000000"">oot</span></p>

<p style=""line-height: normal;text-align: -webkit-center;margin: 0px 0px 1em;color: #8A9499;font-family: Helvetica, Arial, sans-serif;font-size: 11px;"">Have a question?&nbsp;<a href=""http://email.teespring.com/wf/click?upn=HN980Pie9CH3-2BPStnBENhbW4tfqXDfntfzs3iu7STYJ-2BptLRlwqftjDX9u5bwBm9jqJVolsFf8YJ20FUV79uQw-3D-3D_4rgnTcurgLEonc3GE3bj0fnAewlVIvhouLMzi2a7m-2B6796UWSvlGEPGOcSXCNHfESV4NpUiYqXxDIsaizX02YuVrIvtdOBgXAVRwssIdK1EhDDDXg0mKqxk4eu9eqj-2FcJ4lZUOpGVfIL9xwpoZ9c7saFWS4b9v2wFiKveGRnIYmt2AfxY7vbGH-2F7z7RdVcpuVvcVJEpKiujpQoa7zY62qcYyPoszYeJKZpl-2BH00u-2FvLzThb9x0yjBH7fHO1V9tSIw3Sf8G5ARCQGK2YDJscCGTJcVeg7KJuMAf4HkHp585gFWWerrrE6FmvxxJIawJPPVzVSXGCy8-2BIDA-2FQoU6pgYmN4ge-2B2wgeZ3TrtN-2BhhCns5U3XALP633p6l1PEG3Tt8zB69diyuWFUF3T3bIH4n277REgnmdgen7xZciHFzWqO9Kx7JQheXXzpNJfBWNxpa"" style=""color: #5C6366;text-decoration: none;"" target=""_blank"" title=""Send an email to Teespring support""><span style=""color:#000000"">Email</span></a><span style=""color:#000000"">&nbsp;</span>us, or visit our&nbsp;<a href=""http://email.teespring.com/wf/click?upn=HN980Pie9CH3-2BPStnBENhbW4tfqXDfntfzs3iu7STYLEJ-2BD-2FgXhTnxoyDXZuzhZH_4rgnTcurgLEonc3GE3bj0fnAewlVIvhouLMzi2a7m-2B6796UWSvlGEPGOcSXCNHfESV4NpUiYqXxDIsaizX02YuVrIvtdOBgXAVRwssIdK1EhDDDXg0mKqxk4eu9eqj-2FcJ4lZUOpGVfIL9xwpoZ9c7saFWS4b9v2wFiKveGRnIYmt2AfxY7vbGH-2F7z7RdVcpuVvcVJEpKiujpQoa7zY62qcYyPoszYeJKZpl-2BH00u-2FvLzThb9x0yjBH7fHO1V9tSIw3Sf8G5ARCQGK2YDJscCGTJcVeg7KJuMAf4HkHp585jzpRcqIkLslz7CY97M3d-2FcdujbnenAXqdJYad0Dg6jRYWuyxc9DzhWHqJxid-2FV2aG3US1hflAve8Gdq3x0H0qOOINDHbSoqvRu0g7WFATlUZ5S78g-2FcuLX2ugX3FWg-2Bf4SX549M8QANJQ-2BY2U1aXQ7"" style=""color: #5C6366;text-decoration: none;"" target=""_blank"" title=""Teespring Support Center""><span style=""color:#000000"">Support Center</span></a>!</p>

<p style=""line-height: normal;text-align: -webkit-center;margin: 0px 0px 1em;color: #8A9499;font-family: Helvetica, Arial, sans-serif;font-size: 11px;"">&nbsp; Teeyoot, Inc. 3 Davol Square, Providence, RI 02903.&nbsp;</p>

                        </td>
                    </tr>
                </tbody></table>
                
            </td>
        </tr>
    </tbody>
</table><table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""mcnImageBlock"">
    <tbody class=""mcnImageBlockOuter"">
            <tr>
                <td valign=""top"" style=""padding:9px"" class=""mcnImageBlockInner"">
                    <table align=""left"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""mcnImageContentContainer"">
                        <tbody><tr>
                            <td class=""mcnImageContent"" valign=""top"" style=""padding-right: 9px; padding-left: 9px; padding-top: 0; padding-bottom: 0; text-align:center;"">
                                
                                    
                                        <img align=""center"" alt="""" src=""https://gallery.mailchimp.com/9e4bf6e703cbddc2d472096f3/images/c9fd6b84-238e-4a75-9044-76bd0adc41f1.png"" width=""200"" style=""max-width:200px; padding-bottom: 0; display: inline !important; vertical-align: bottom;"" class=""mcnImage"">
                                    
                                
                            </td>
                        </tr>
                    </tbody></table>
                </td>
            </tr>
    </tbody>
</table></td>
                                        </tr>
                                    </table>
                                    <!-- // END FOOTER -->
                                </td>
                            </tr>
                        </table>
                        <!-- // END TEMPLATE -->
                    </td>
                </tr>
            </table>
        </center>
    </body>
</html>
        ";
}
        
    }



}