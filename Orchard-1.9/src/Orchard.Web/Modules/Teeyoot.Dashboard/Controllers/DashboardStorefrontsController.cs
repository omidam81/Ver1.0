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
            var storeFront = _storeService.GetStoreByUrl(url);
            var campaigns = _campaignService.GetCampaignsOfUser(int.Parse(storeFront.TeeyootUserId.ToString())).ToList();
            var model = new StoreViewModel();
            model.Campaigns = campaigns;

            var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), storeFront.TeeyootUserId.ToString(), storeFront.Id.ToString());
            DirectoryInfo dir = new DirectoryInfo(destForder);

            foreach (FileInfo fi in dir.GetFiles())
            {
                model.Img = "/Media/Storefronts/" + storeFront.TeeyootUserId.ToString() + "/" + storeFront.Id.ToString() + "/"+ fi.Name;
            }

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
            var user = _wca.GetContext().CurrentUser;
            
                if (user != null)
                {
                    var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));

                    if (teeyootUser.Id == storeFront.TeeyootUserId)
                    {
                        return View(model);
                    }
                    else
                    {

                        if (!storeFront.HideStore)
                        {
                            return View("StorefrontForClient", model);
                        }
                        else
                        {
                            return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
                        }
                    }
                }
                else
                {
                    if (!storeFront.HideStore)
                    {
                        return View("StorefrontForClient", model);
                    }
                    else
                    {
                        return View("NotFound", Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "");
                    }
                    
                }
           
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
                
                clearFolder(destForder);

                if (!Directory.Exists(destForder))
                {
                    Directory.CreateDirectory(destForder);
                }

                _imageHelper.Base64ToBitmap(base64image).Save(Path.Combine(destForder, "storefront"+DateTime.Now.Millisecond+".png"), ImageFormat.Png);
            }

            Response.Write(url);
            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }


        [HttpPost]
        public HttpStatusCodeResult DeleteStorefront(int id)
        {  
            _storeService.DeleteStore(id);

            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
          
            var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), teeyootUser.Id.ToString());

            clearFolder(destForder);
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
        

    }
}