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

        public ActionResult Shop()
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
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            var cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            var campaigns = _campaignService.GetCampaignsOfUser(teeyootUser.Id).Where(c => c.IsApproved && c.CampaignCulture == cultureUsed).ToList();
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, T("Such url exists! It must be unique.").ToString());
                }
            }
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var newStore = _storeService.CreateStore(teeyootUser.Id, title,  description,url, hideStore, crossSelling, selectedCampaigns);

            if (base64image != "")
            {
                var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), teeyootUser.Id.ToString(), newStore.Id.ToString());

                if (!Directory.Exists(destForder))
                {
                    Directory.CreateDirectory(destForder);
                }

                _imageHelper.Base64ToBitmap(base64image).Save(Path.Combine(destForder, "storefront.png"), ImageFormat.Png);

            }
  
            Response.Write(url);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
            
        }

 
        public ActionResult ViewStorefront(string url)
        {
            var culture = _wca.GetContext().CurrentCulture.Trim();
            var cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            var storeFront = _storeService.GetStoreByUrl(url);
            if (storeFront.Campaigns[0].CampaignRecord.CampaignCulture != cultureUsed)
            {
                _cookieCultureService.SetCulture(storeFront.Campaigns[0].CampaignRecord.CampaignCulture);
                return RedirectToAction("ViewStorefront", new { url = url });
            }
            var campaigns = _campaignService.GetCampaignsOfUser(int.Parse(storeFront.TeeyootUserId.ToString())).Where(c => c.IsApproved).ToList();
            var model = new StoreViewModel();
            model.Campaigns = campaigns;

            var destFolder = Path.Combine(Server.MapPath("/Media/Storefronts/"), storeFront.TeeyootUserId.ToString(), storeFront.Id.ToString());
            DirectoryInfo dir = new DirectoryInfo(destFolder);

            if (dir.Exists == true)
            {
                foreach (FileInfo fi in dir.GetFiles())
                {
                    model.Img = "/Media/Storefronts/" + storeFront.TeeyootUserId.ToString() + "/" + storeFront.Id.ToString() + "/" + fi.Name;
                }
            }
            if (model.Img == null)
            {
                model.Img = "/Media/Default/images/storefront.png";
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
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, T("Such url exists! It must be unique.").ToString());
                }
            }
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));

            _storeService.UpdateStore(id, teeyootUser.Id, title, description, url, hideStore, crossSelling, selectedCampaigns);

            if (base64image != null)
            {
                var destForder = Path.Combine(Server.MapPath("/Media/Storefronts/"), teeyootUser.Id.ToString(), id.ToString());

                DirectoryInfo dir = new DirectoryInfo(destForder);
                if (dir.Exists == true)
                {
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        fi.Delete();
                    }
                }
                else
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

            ClearFolder(destForder, id.ToString());
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void ClearFolder(string FolderPath, string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderPath);
            if (dir.Exists == true)
            {               
                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    if (di.Name == FolderName)
                    {
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            fi.Delete();
                        }

                        di.Delete();
                    }
                }
            }
        }
        

    }
}