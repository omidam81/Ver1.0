using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services;
using Teeyoot.Messaging.ViewModels;
using Mandrill;
using Mandrill.Model;
using System.Collections.Specialized;


namespace Teeyoot.Module.Controllers
{
    [Admin]
    public class AdminMessageController : Controller, IUpdateModel
    {
        private readonly IMailChimpSettingsService _settingsService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private string cultureUsed = string.Empty;

        public AdminMessageController(IMailChimpSettingsService settingsService, IOrchardServices services, IWorkContextAccessor workContextAccessor)
        {
            _settingsService = settingsService;
            Services = services;
            _workContextAccessor = workContextAccessor;
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
        }

        private IOrchardServices Services { get; set; }
        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public ActionResult Index()
        {
            var pathToTemplates = Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/");
            var settings = _settingsService.GetSettingByCulture(cultureUsed).List().Select(s => new MailChimpListViewModel
            {
                Id = s.Id,
                ApiKey = s.ApiKey,
                SellerTemplate = System.IO.File.Exists(pathToTemplates + "seller-template-" + cultureUsed + ".html") ? "seller-template-" + cultureUsed + ".html" : "No file!",
                WelcomeTemplate = System.IO.File.Exists(pathToTemplates + "welcome-template-" + cultureUsed + ".html") ? "welcome-template-" + cultureUsed + ".html" : "No file!",
                RelaunchApprovedSellerTemplate = System.IO.File.Exists(pathToTemplates + "relaunch-" + cultureUsed + ".html") ? "relaunch-" + cultureUsed + ".html" : "No file!",
                RelaunchApprovedBuyerTemplate = System.IO.File.Exists(pathToTemplates + "relaunch-buyer-" + cultureUsed + ".html") ? "relaunch-buyer-" + cultureUsed + ".html" : "No file!",
                RelaunchAdminSellerTemplate = System.IO.File.Exists(pathToTemplates + "relaunch-to-admin-seller-" + cultureUsed + ".html") ? "relaunch-to-admin-seller-" + cultureUsed + ".html" : "No file!",
                LaunchTemplate = System.IO.File.Exists(pathToTemplates + "launch-template-" + cultureUsed + ".html") ? "launch-template-" + cultureUsed + ".html" : "No file!",
                WithdrawTemplate = System.IO.File.Exists(pathToTemplates + "withdraw-template-" + cultureUsed + ".html") ? "withdraw-template-" + cultureUsed + ".html" : "No file!",
                PlaceOrderTemplate = System.IO.File.Exists(pathToTemplates + "place-order-template-" + cultureUsed + ".html") ? "place-order-template-" + cultureUsed + ".html" : "No file!",
                NewOrderTemplate = System.IO.File.Exists(pathToTemplates + "new-order-template-" + cultureUsed + ".html") ? "new-order-template-" + cultureUsed + ".html" : "No file!",
                ShippedOrderTemplate = System.IO.File.Exists(pathToTemplates + "shipped-order-template-" + cultureUsed + ".html") ? "shipped-order-template-" + cultureUsed + ".html" : "No file!",
                DeliveredOrderTemplate = System.IO.File.Exists(pathToTemplates + "delivered-order-template-" + cultureUsed + ".html") ? "delivered-order-template-" + cultureUsed + ".html" : "No file!",
                CancelledOrderTemplate = System.IO.File.Exists(pathToTemplates + "cancelled-order-template-" + cultureUsed + ".html") ? "cancelled-order-template-" + cultureUsed + ".html" : "No file!",
                OrderIsPrintingBuyerTemplate = System.IO.File.Exists(pathToTemplates + "order-is-printing-buyer-template-" + cultureUsed + ".html") ? "order-is-printing-buyer-template" + cultureUsed + ".html" : "No file!",
                CampaignIsPrintingSellerTemplate = System.IO.File.Exists(pathToTemplates + "campaign-is-printing-seller-template-" + cultureUsed + ".html") ? "campaign-is-printing-seller-template-" + cultureUsed + ".html" : "No file!",
                PaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "paid-campaign-template-" + cultureUsed + ".html") ? "paid-campaign-template-" + cultureUsed + ".html" : "No file!",
                UnpaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "unpaid-campaign-template-" + cultureUsed + ".html") ? "unpaid-campaign-template-" + cultureUsed + ".html" : "No file!",
                CampaignNotReachGoalBuyerTemplate = System.IO.File.Exists(pathToTemplates + "not-reach-goal-seller-template-" + cultureUsed + ".html") ? "not-reach-goal-seller-template-" + cultureUsed + ".html" : "No file!",
                CampaignNotReachGoalSellerTemplate = System.IO.File.Exists(pathToTemplates + "not-reach-goal-buyer-template-" + cultureUsed + ".html") ? "not-reach-goal-buyer-template-" + cultureUsed + ".html" : "No file!",
                PartiallyPaidCampaignTemplate = System.IO.File.Exists(pathToTemplates + "partially-paid-campaign-template-" + cultureUsed + ".html") ? "partially-paid-campaign-template-" + cultureUsed + ".html" : "No file!",
                CampaignPromoTemplate = System.IO.File.Exists(pathToTemplates + "campaign-promo-template-" + cultureUsed + ".html") ? "campaign-promo-template-" + cultureUsed + ".html" : "No file!",
                AllOrderDeliveredTemplate = System.IO.File.Exists(pathToTemplates + "all-orders-delivered-seller-template-" + cultureUsed + ".html") ? "all-orders-delivered-seller-template-" + cultureUsed + ".html" : "No file!",
                CampaignIsFinishedTemplate = System.IO.File.Exists(pathToTemplates + "campaign-is-finished-template-" + cultureUsed + ".html") ? "campaign-is-finished-template-" + cultureUsed + ".html" : "No file!",
                DefinitelyGoSellerTemplate = System.IO.File.Exists(pathToTemplates + "definitely-go-to-print-buyer-template-" + cultureUsed + ".html") ? "definitely-go-to-print-buyer-template-" + cultureUsed + ".html" : "No file!",
                DefinitelyGoBuyerTemplate = System.IO.File.Exists(pathToTemplates + "definitely-go-to-print-seller-template-" + cultureUsed + ".html") ? "definitely-go-to-print-seller-template-" + cultureUsed + ".html" : "No file!",
                EditedCampaignTemplate = System.IO.File.Exists(pathToTemplates + "edited-campaign-template-" + cultureUsed + ".html") ? "edited-campaign-template-" + cultureUsed + ".html" : "No file!",
                ExpiredMetMinimumTemplate = System.IO.File.Exists(pathToTemplates + "expired-campaign-met-minimum-admin-template-" + cultureUsed + ".html") ? "expired-campaign-met-minimum-admin-template-" + cultureUsed + ".html" : "No file!",
                ExpiredNotSuccessfullTemplate = System.IO.File.Exists(pathToTemplates + "expired-campaign-notSuccessfull-admin-template-" + cultureUsed + ".html") ? "expired-campaign-notSuccessfull-admin-template-" + cultureUsed + ".html" : "No file!",
                ExpiredSuccessfullTemplate = System.IO.File.Exists(pathToTemplates + "expired-campaign-successfull-admin-template-" + cultureUsed + ".html") ? "expired-campaign-successfull-admin-template-" + cultureUsed + ".html" : "No file!",
                MakeTheCampaignTemplate = System.IO.File.Exists(pathToTemplates + "make_the_campaign_seller-" + cultureUsed + ".html") ? "make_the_campaign_seller-" + cultureUsed + ".html" : "No file!",
                NewCampaignAdminTemplate = System.IO.File.Exists(pathToTemplates + "new-campaign-admin-template-" + cultureUsed + ".html") ? "new-campaign-admin-template-" + cultureUsed + ".html" : "No file!",
                NewOrderBuyerTemplate = System.IO.File.Exists(pathToTemplates + "new-order-buyer-template-" + cultureUsed + ".html") ? "new-order-buyer-template-" + cultureUsed + ".html" : "No file!",
                NotReachGoalMetMinimumTemplate = System.IO.File.Exists(pathToTemplates + "not-reach-goal-met-minimum-seller-template-" + cultureUsed + ".html") ? "not-reach-goal-met-minimum-seller-template-" + cultureUsed + ".html" : "No file!",
                RecoverOrdersTemplate = System.IO.File.Exists(pathToTemplates + "recover_orders_for_buyer-" + cultureUsed + ".html") ? "recover_orders_for_buyer-" + cultureUsed + ".html" : "No file!",
                RejectTemplate = System.IO.File.Exists(pathToTemplates + "reject-template-" + cultureUsed + ".html") ? "reject-template-" + cultureUsed + ".html" : "No file!",
                Shipped3DayAfterTemplate = System.IO.File.Exists(pathToTemplates + "shipped-order-3day-after-template-" + cultureUsed + ".html") ? "shipped-order-3day-after-template-" + cultureUsed + ".html" : "No file!",
                TermsConditionsTemplate = System.IO.File.Exists(pathToTemplates + "terms-conditions-template-" + cultureUsed + ".html") ? "terms-conditions-template-" + cultureUsed + ".html" : "No file!",
                WithdrawCompletedTemplate = System.IO.File.Exists(pathToTemplates + "withdraw-completed-template-" + cultureUsed + ".html") ? "withdraw-completed-template-" + cultureUsed + ".html" : "No file!",
                WithdrawSellerTemplate = System.IO.File.Exists(pathToTemplates + "withdraw-seller-template-" + cultureUsed + ".html") ? "withdraw-seller-template-" + cultureUsed + ".html" : "No file!"
            });
            if (settings.FirstOrDefault() == null)
            {
                var settingPart = new MailChimpListViewModel();
                return View(settingPart);
            }
                     
            return View(settings.FirstOrDefault());
        }

        public ActionResult AddSetting(string returnUrl)
        {
            MailChimpSettingsPart mailChimpSettingsPart = Services.ContentManager.New<MailChimpSettingsPart>("MailChimpSettings");

            if (mailChimpSettingsPart == null)
                return HttpNotFound();
            try
            {
                var model = Services.ContentManager.BuildEditor(mailChimpSettingsPart);
                return View(model);
            }
            catch (Exception exception)
            {
                Logger.Error(T("Creating setting failed: {0}", exception.Message).Text);
                Services.Notifier.Error(T("Creating setting failed: {0}", exception.Message));
                return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
            }
        }

        [HttpPost, ActionName("AddSetting")]
        public ActionResult AddSettingPOST(string returnUrl)
        {
            Uri myUri = new Uri(returnUrl);
            var mailChimpSettingPart = _settingsService.CreateMailChimpSettingsPart("",cultureUsed);
            if (mailChimpSettingPart == null)
                return HttpNotFound();

            var model = Services.ContentManager.UpdateEditor(mailChimpSettingPart, this);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    Services.Notifier.Error(T(error));
                }
                return View(model);
            }

            Services.Notifier.Information(T("New setting has been added."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }


        public ActionResult EditMailChimpSetting(int id)
        {
            var mailChimpSettingPart = _settingsService.GetSetting(id);
            if (mailChimpSettingPart == null)
                return new HttpNotFoundResult();

            var model = Services.ContentManager.BuildEditor(mailChimpSettingPart);
            return View(model);
        }

        [HttpPost, ActionName("EditMailChimpSetting")]
        public ActionResult EditMailChimpSettingPOST(int id, FormCollection input, string returnUrl)
        {
            var mailChimpSettingPart = _settingsService.GetSetting(id);

            var model = Services.ContentManager.UpdateEditor(mailChimpSettingPart, this);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(m => m.Errors).Select(e => e.ErrorMessage))
                {
                    Services.Notifier.Error(T(error));
                }
                return View(model);
            }

            Services.Notifier.Information(T("The setting has been updated."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        public ActionResult Delete(string templateName, string returnUrl)
        {
            System.IO.File.Delete(Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/") + templateName +"-" + cultureUsed + ".html");
            Services.Notifier.Information(T("File has been deleted."));
            return this.RedirectLocal(returnUrl, () => RedirectToAction("Index"));
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        [HttpPost]
        public RedirectToRouteResult UploadTemplate(HttpPostedFileBase file, string templateName)
        {
            if (file != null && file.ContentLength > 0)
            {
                string[] allowed = { ".html" };
                var extension = System.IO.Path.GetExtension(file.FileName);
                if (allowed.Contains(extension))
                {
                    string fileExt = Path.GetExtension(file.FileName);
                    var path = Path.Combine(Server.MapPath("/Modules/Teeyoot.Module/Content/message-templates/"), templateName + "-" + cultureUsed + fileExt);
                    file.SaveAs(path);
                    Services.Notifier.Information(T("File has been added!"));
                    return RedirectToAction("Index");
                }
            }
            Services.Notifier.Error(T("Wrong file extention!"));
            return RedirectToAction("Index");
        }

        public void Download(string fileName)
        {
            fileName = fileName + "-" + cultureUsed + ".html";
            string pathToMedia = AppDomain.CurrentDomain.BaseDirectory;
            string pathToTemplates = Path.Combine(pathToMedia, "Modules/Teeyoot.Module/Content/message-templates/");
            Response.ContentType = "text/HTML";
            String Header = "Attachment; Filename=" + fileName;
            Response.AppendHeader("Content-Disposition", Header);
            System.IO.FileInfo Dfile = new System.IO.FileInfo(pathToTemplates+fileName);
            Response.WriteFile(Dfile.FullName);
            Response.End();
        }
        
    }
}