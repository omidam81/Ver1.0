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
            string currentUser = Services.WorkContext.CurrentUser.Email;
            var user = _membershipService.GetUser(currentUser);
            var campaigns = _campaignService.GetCampaignsOfUser(user.Id)
                .Select(c => new MessagesCampaignViewModel
                {
                    Title = c.Title,
                    Id = c.Id,
                    Sold = c.ProductCountSold
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
            model.ProductId = campaign.Products[0].Id;
            model.CampaignTitle = campaign.Title;
            return View(model);
        }

        public ActionResult SendSellerMessageForApproving(MessageContentViewModel model)
        {
            if (TryUpdateModel(model))
            {
                string currentUser = Services.WorkContext.CurrentUser.Email;
                var user = _membershipService.GetUser(currentUser);
                _messageService.AddMessage(user.Id, model.Content, model.From, DateTime.UtcNow, model.CampaignId, model.Subject, false);
                _notifier.Information(T("Your message has been sent for approving!"));
                return RedirectToAction("Messages");
            }
            return View("CreateMessage", model);
        }


        
    }



}