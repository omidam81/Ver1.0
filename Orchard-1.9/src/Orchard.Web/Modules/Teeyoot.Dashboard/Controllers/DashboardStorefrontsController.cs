using System.Web.Mvc;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Net;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {

        public ActionResult Storefronts()
        {
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var stores = _storeService.GetAllStoresForUser(teeyootUser.Id).ToList();
            return View(stores);
        }

        public ActionResult NewStorefront()
        {
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var campaigns = _campaignService.GetCampaignsOfUser(teeyootUser.Id).ToList();
            var model = new StoreViewModel();
            model.Campaigns = campaigns;
            return View(model);
        }

        [HttpPost]
        public HttpStatusCodeResult CreateStorefront(string base64image, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns)
        {
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (store.Url == url)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Such url exists! It must be unique.");
                }
            }
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var newStore = _storeService.CreateStore(teeyootUser.Id, title,  description,url, hideStore, crossSelling, selectedCampaigns);


            var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), teeyootUser.Id.ToString(), newStore.Id.ToString());

            if (!Directory.Exists(destForder))
            {
                Directory.CreateDirectory(destForder);
            }

            _imageHelper.Base64ToBitmap(base64image).Save(Path.Combine(destForder, "storefront.png"), ImageFormat.Png);

            
            Response.Write(url);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
            
        }
        public ActionResult ViewStorefront(string url)
        {
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var storeFront = _storeService.GetStoreByUrl(url);

            var campaigns = _campaignService.GetCampaignsOfUser(teeyootUser.Id).ToList();
            var model = new StoreViewModel();
            model.Campaigns = campaigns;
            model.Img = "/Media/Storefronts/" + teeyootUser.Id.ToString() + "/" + storeFront.Id.ToString() + "/storefront.png";
            model.Title = storeFront.Title;
            model.Description = storeFront.Description;
            IList<CampaignRecord> selectedCamp = new List<CampaignRecord>();
            foreach (var camp in storeFront.Campaigns)
            {
                selectedCamp.Add(camp.CampaignRecord);
            }
            model.SelectedCampaigns = selectedCamp;
            model.CrossSelling = storeFront.CrossSelling;
            model.HideStore = storeFront.HideStore;
            model.Url = storeFront.Url;
            model.Id = storeFront.Id;
            return View(model);
        }

        [HttpPost]
        public HttpStatusCodeResult SaveStorefront(string base64image, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns, int id)
        {
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (store.Url == url & store.Id != id)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Such url exists! It must be unique.");
                }
            }
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));

            _storeService.UpdateStore(id, teeyootUser.Id, title, description, url, hideStore, crossSelling, selectedCampaigns);

            if (base64image != null)
            {
                var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), teeyootUser.Id.ToString(), id.ToString());

                if (!Directory.Exists(destForder))
                {
                    Directory.CreateDirectory(destForder);
                }

                _imageHelper.Base64ToBitmap(base64image).Save(Path.Combine(destForder, "storefront.png"), ImageFormat.Png);
            }

            Response.Write(url);
            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }


        [HttpPost]
        public HttpStatusCodeResult DeleteStorefront(int id)
        {
            _storeService.DeleteStore(id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        

    }
}