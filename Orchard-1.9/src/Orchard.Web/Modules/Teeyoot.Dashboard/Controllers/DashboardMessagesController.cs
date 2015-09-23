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
using Teeyoot.Module.Dashboard.ViewModels;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        // GET: Message
        public ActionResult Messages()
        {
            int currentUser = Services.WorkContext.CurrentUser.Id;
            var campaigns = _campaignService.GetCampaignsOfUser(currentUser)
                .Select(c => new MessagesCampaignViewModel
                {
                    Title = c.Title,
                    Id = c.Id,
                    Sold = c.ProductCountSold,
                    BackByDefault = c.BackSideByDefault
                })
                .ToList();

            var campaignProducts = _campaignService.GetAllCampaignProducts();

            List<MessagesIndexViewModel> listModel = new List<MessagesIndexViewModel>();
            foreach (var item in campaigns)
            {                
                var tempModel = new MessagesIndexViewModel() { };
                tempModel.ThisWeekSend = _messageService.GetAllMessagesForCampaign(item.Id).Where(s => (s.SendDate < DateTime.UtcNow) && (s.SendDate > DateTime.UtcNow.AddDays(-7))).Count();
                if (_messageService.GetLatestMessageDateForCampaign(item.Id).Day > DateTime.UtcNow.Day) 
                {
                    tempModel.LastSend = T("Never").ToString();
                }
                else
                {
                    if (DateTime.UtcNow.Day - _messageService.GetLatestMessageDateForCampaign(item.Id).Day == 0)
                    {
                        tempModel.LastSend = T("Today").ToString();
                    }
                    else
                    {
                        tempModel.LastSend = (DateTime.UtcNow.Day - _messageService.GetLatestMessageDateForCampaign(item.Id).Day).ToString() + T(" days ago").ToString();
                    }
                }

                var prods = campaignProducts.FirstOrDefault(p => p.CampaignRecord_Id == item.Id);
                item.FirstProductId = prods != null ? prods.Id : 0;

                tempModel.Campaign = item;
                listModel.Add(tempModel);

            }
            //IEnumerable<MessagesIndexViewModel> model = listModel;
            return View(listModel);
        }


        public ActionResult CreateMessage(int campaignId)
        {
            MessageContentViewModel model = new MessageContentViewModel() { };
            model.CampaignId = campaignId;
            var campaign = _campaignService.GetCampaignById(campaignId);
            model.ProductId = campaign.Products.Where(pr=>pr.WhenDeleted == null).First().Id;
            model.CampaignTitle = campaign.Title;
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult SendSellerMessageForApproving(MessageContentViewModel model)
        {
            if (TryUpdateModel(model))
            {
                int currentUser = Services.WorkContext.CurrentUser.Id;
                _messageService.AddMessage(currentUser, model.Content, model.From, DateTime.UtcNow, model.CampaignId, model.Subject, false);
                _notifier.Information(T("Your message has been sent for approving!"));
                return RedirectToAction("Messages");
            }
            return View("CreateMessage", model);
        }

        public JsonResult GetCampaignUrlAndImagePath(string campAlias)
        {
            var campaign = _campaignService.GetCampaignByAlias(campAlias);
            if (campaign != null)
            {
                string baseUrl = "";
                if (System.Web.HttpContext.Current != null)
                {
                    var request = System.Web.HttpContext.Current.Request;
                    baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
                }
                else
                {
                    baseUrl = _wca.GetContext().CurrentSite.BaseUrl + "/";
                }
                string side = "";
                if (campaign.BackSideByDefault)
                {
                    side = "back";
                }
                else
                {
                    side = "front";
                }
                string campaignUrl = baseUrl + campaign.Title;
                string imagePath = baseUrl + "/Media/campaigns/" + campaign.Id + "/" + campaign.Products.First(p => p.WhenDeleted == null).Id + "/normal/" + side + ".png";
                return Json(new { campaignUrl = campaignUrl, imagePath = imagePath }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { campaignUrl = "", imagePath = "" }, JsonRequestBehavior.AllowGet);
        }
        
    }



}