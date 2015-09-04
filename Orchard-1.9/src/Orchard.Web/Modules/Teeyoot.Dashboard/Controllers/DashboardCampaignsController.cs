using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.ExtentionMethods;
using System;
using System.Threading.Tasks;
using Orchard.Themes;
using Orchard;
using Orchard.Mvc.Routes;
using System.Web.Routing;
using Orchard.UI.Notify;
using System.Net;
using Teeyoot.Module.ViewModels;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using Orchard.Data;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        //public async Task<ActionResult> Campaigns()
        //{
        //    var model = new CampaignsViewModel();
        //    model.Currency = "RM"; //TODO: eugene: implement currency
        //    var user = _wca.GetContext().CurrentUser;
        //    var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
        //    var campaignsQuery = _campaignService.GetCampaignsOfUser(teeyootUser != null ? teeyootUser.Id : 0);
        //    var productsOrderedQuery = _orderService.GetProductsOrderedOfCampaigns(campaignsQuery.Select(c => c.Id).ToArray());

        //    await FillCampaigns(model, campaignsQuery);
        //    FillOverviews(model, productsOrderedQuery, campaignsQuery);

        //    return View(model);
        //}

        public ActionResult Campaigns(bool? isError, string result)
        {
            var model = new CampaignsViewModel();
            model.MYCurrencyCode = _currencyRepository.Table.Where(c=> c.CurrencyCulture == "en-MY").FirstOrDefault().Code; 
            model.IDCurrencyCode = _currencyRepository.Table.Where(c=> c.CurrencyCulture == "id-ID").FirstOrDefault().Code;
            model.SGCurrencyCode = _currencyRepository.Table.Where(c => c.CurrencyCulture == "en-SG").FirstOrDefault().Code; 
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var campaignsQuery = _campaignService.GetCampaignsOfUser(teeyootUser != null ? teeyootUser.Id : 0);
            var productsOrderedQuery = _orderService
                .GetProductsOrderedOfCampaigns(campaignsQuery.Select(c => c.Id).ToArray());

            FillCampaigns(model, campaignsQuery);
            FillOverviews(model, productsOrderedQuery, campaignsQuery);
          
            return View(model);
        }

        //private async Task FillCampaigns(CampaignsViewModel model, IQueryable<CampaignRecord> campaignsQuery)
        //{
        //    var campaignSummaries = new List<CampaignSummary>();
        //    var campaigns = campaignsQuery.OrderBy(c => c.StartDate).ToList();

        //    foreach (var c in campaigns)
        //    {
        //        campaignSummaries.Add(new CampaignSummary
        //        {
        //            Alias = c.Alias,
        //            EndDate = c.EndDate,
        //            Goal = c.ProductCountGoal,
        //            Id = c.Id,
        //            Name = c.Title,
        //            Sold = c.ProductCountSold,
        //            StartDate = c.StartDate,
        //            Status = c.CampaignStatusRecord,
        //            ShowBack = c.BackSideByDefault,
        //            FirstProductId = c.Products[0].Id,
        //            Profit = await _orderService.GetProfitOfCampaign(c.Id)                   
        //        });
        //    }
        //    model.Campaigns = campaignSummaries;
        //}

        private void FillCampaigns(CampaignsViewModel model, IQueryable<CampaignRecord> campaignsQuery)
        {
            var campaignProducts = _campaignService.GetAllCampaignProducts();
            var orderedProducts = _orderService.GetAllOrderedProducts();

            var campaignSummaries = campaignsQuery
                .Select(c => new CampaignSummary
                    {
                        Alias = c.Alias,
                        EndDate = c.EndDate,
                        Goal = c.ProductCountGoal,
                        Id = c.Id,
                        Name = c.Title,
                        Sold = c.ProductCountSold,
                        Minimum=c.ProductMinimumGoal,
                        StartDate = c.StartDate,
                        Status = c.CampaignStatusRecord,
                        IsActive = c.IsActive,
                        IsArchived = c.IsArchived,
                        ShowBack = c.BackSideByDefault,
                        IsPrivate = c.IsPrivate
                    })
                .OrderBy(c => c.StartDate)
                .ToArray();

            foreach (var item in campaignSummaries)
            {
                item.CountRequests = _campaignService.GetCountOfReservedRequestsOfCampaign(item.Id);
                var prods = campaignProducts.Where(c => c.WhenDeleted == null).FirstOrDefault(p => p.CampaignRecord_Id == item.Id);
                item.FirstProductId = prods != null ? prods.Id : 0;
                item.Profit = orderedProducts
                                    .Where(p => p.OrderRecord.IsActive && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.CampaignProductRecord.CampaignRecord_Id == item.Id)
                                    .Select(pr => new { Profit = pr.Count * (pr.CampaignProductRecord.Price - pr.CampaignProductRecord.BaseCost) })
                                    .Sum(entry => (double?)entry.Profit) ?? 0;
                item.SummaryCurrency = _currencyRepository.Table.Where(c => c.CurrencyCulture == (_campaignService.GetCampaignById(item.Id).CampaignCulture)).FirstOrDefault().Code; 
            }

            campaignSummaries = campaignSummaries.Where(c => c.FirstProductId > 0).ToArray();

            model.Campaigns = campaignSummaries;
        }

        private void FillOverviews(CampaignsViewModel model, IQueryable<LinkOrderCampaignProductRecord> productsOrderedQuery, IQueryable<CampaignRecord> campaignsQuery)
        {
            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.Today,
                ProductsOrdered = productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                            .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled")
                            .Sum(p => (int?)p.Count) ?? 0,
                MYProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                             .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                SGProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                             .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-SG")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                IDProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                             .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "id-ID")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2)
                //,
                //ToBePaid = productsOrderedQuery
                //            .FilterByType(OverviewType.Today)
                //            .Where(p => !p.OrderRecord.Reserved.HasValue)
                //            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                //            .Sum(entry => entry.Profit)
            });

            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.Yesterday,
                ProductsOrdered = productsOrderedQuery
                            .FilterByType(OverviewType.Yesterday)
                            .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled")
                            .Sum(p => (int?)p.Count) ?? 0,
                MYProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Yesterday)
                             .Where(p =>  p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                SGProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Yesterday)
                             .Where(p =>  p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-SG")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                IDProfit = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Yesterday)
                             .Where(p =>  p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "id-ID")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2)
                //,
                //ToBePaid = productsOrderedQuery
                //            .FilterByType(OverviewType.Yesterday)
                //            .Where(p => !p.OrderRecord.Reserved.HasValue)
                //            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                //            .Sum(entry => entry.Profit)
            });

            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.Active,
                ProductsOrdered = productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled")
                            .Sum(p => (int?)p.Count) ?? 0,
                MYProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2),
                SGProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-SG")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2),
                IDProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "id-ID")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2),
                MYToBePaid = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => !p.OrderRecord.Paid.HasValue  && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                SGToBePaid = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => !p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-SG")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2),
                IDToBePaid = Math.Round(productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => !p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "id-ID")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0,2)
            });

            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.AllTime,
                ProductsOrdered = productsOrderedQuery
                             .Where(p => p.OrderRecord.OrderStatusRecord.Name != "Cancelled")
                            .Sum(p => (int?)p.Count) ?? 0,
                MYProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-MY")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2),
                SGProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "en-SG")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2),
                IDProfit =  Math.Round(productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue && p.OrderRecord.OrderStatusRecord.Name != "Cancelled" && p.OrderRecord.CurrencyRecord.CurrencyCulture == "id-ID")
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (double?)entry.Profit) ?? 0, 2)
                //,
                //ToBePaid = productsOrderedQuery
                //            .Where(p => !p.OrderRecord.Reserved.HasValue)
                //            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                //            .Sum(entry => entry.Profit)
            });
        }

        [Themed]
        [Authorize]
        public ActionResult EditCampaign(int id)
        {
            CampaignRecord camp = _campaignService.GetCampaignById(id);
            var user = Services.WorkContext.CurrentUser;
            if (camp.TeeyootUserId != user.Id)
            {
                return View("EditCampaign", new EditCampaignViewModel { IsError = true });
            }

            var tags = _campaignCategoryService.GetCategoryByCampaignId(camp.Id).ToList();
            string allTags = string.Empty;
            foreach (var tag in tags)
            {
                allTags = allTags + " " + tag.Name;
            }
            int product = _campaignService.GetProductsOfCampaign(id).Where(c => c.WhenDeleted == null).First().Id;

            string path = "/Media/campaigns/" + camp.Id.ToString() + "/" + product.ToString() + "/";
            string backIMG;
            string frontIMG;
            if (camp.BackSideByDefault)
            {
                backIMG = path + "normal/front.png";
                frontIMG = path + "normal/back.png";
            }
            else
            {
                backIMG = path + "normal/back.png";
                frontIMG = path + "normal/front.png";
            }

            return View("EditCampaign", new EditCampaignViewModel { IsError = false, Id = camp.Id, Title = camp.Title, Description = camp.Description, Tags = allTags, Alias = camp.Alias, BackSideByDefault = camp.BackSideByDefault, FrontImagePath = frontIMG, BackImagePath = backIMG });
        }

        public ActionResult SaveChanges(EditCampaignViewModel editCampaign)
        {
            var campaign = _campaignService.GetCampaignById(editCampaign.Id);

            campaign.Title = editCampaign.Title;
            campaign.Description = editCampaign.Description;
            campaign.Alias = editCampaign.Alias;
            campaign.BackSideByDefault = editCampaign.BackSideByDefault;

            var tags = _campaignCategoryService.GetCategoryByCampaignId(editCampaign.Id).ToList();
            var allTags = _campaignCategoryService.GetAllCategories();

            List<string> campaignTags = editCampaign.Tags.Split(' ').ToList();

            List<string> campaignTagsForeach = campaignTags.ToList();
            List<CampaignCategoriesRecord> tagsInBD = new List<CampaignCategoriesRecord>();

            foreach (var tag in campaignTagsForeach)
            {
                var newTag = tags.Where(c => c.Name.ToLower() == tag.ToLower()).FirstOrDefault();
                if (newTag != null)
                {
                    campaignTags.Remove(newTag.Name);
                }

                newTag = allTags.Where(c => c.Name.ToLower() == tag.ToLower()).FirstOrDefault();
                if (newTag != null)
                {
                    campaignTags.Remove(newTag.Name);
                    tagsInBD.Add(newTag);
                }
            }

            List<CampaignCategoriesRecord> newTags = new List<CampaignCategoriesRecord>();
            foreach (var tag in campaignTags)
            {
                CampaignCategoriesRecord newTag = new CampaignCategoriesRecord
                {
                    Name = tag,
                    IsVisible = false
                };
                newTags.Add(newTag);
            }

            if (_campaignCategoryService.UpdateCampaignAndCreateNewCategories(campaign, newTags, tagsInBD))
            {
                _notifier.Information(T("Campaign was updated successfully"));
                return RedirectToAction("Campaigns");
            }
            else
            {
                _notifier.Error(T("An error occurred when updating the campaign. Try again."));
                return RedirectToAction("EditCampaign", new { id = editCampaign.Id });
            }
        }

        public JsonResult GetDetailTags(string filter)
        {
            int filterNull = filter.LastIndexOf(' ');
            if (filterNull == filter.Length - 1)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            string[] filters = filter.Split(' ');
            string tag = filters[filters.Length - 1];

            var entries = _campaignService.GetAllCategories().Where(c => c.Name.Contains(tag)).Select(n => n.Name).Take(10).ToList();
            return Json(entries, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Wizard(int id)
        {
            return RedirectToAction("Index", "Wizard", new RouteValueDictionary {
                            {"id", id},
                            {"area", "Teeyoot.Module"},
                            {"controller", "Wizard"},
                            {"action", "Index"}                           
                        });
        }

        public ActionResult DeleteCampaign(int id)
        {
            if (_campaignService.DeleteCampaign(id))
            {
                _notifier.Information(T("The campaign was deleted successfully!"));
            }
            else
            {
                _notifier.Error(T("The company could not be removed. Try again!"));
            }

            return RedirectToAction("Campaigns");
        }

        public ActionResult PrivateCampaign(int id, bool change)
        {
            if (_campaignService.PrivateCampaign(id, change))
            {
                if (change)
                {
                    _notifier.Information(T("Campaign set status - private"));
                }
                else
                {
                    _notifier.Information(T("Campaign set status - public"));
                }
            }
            else
            {
                _notifier.Error(T("The company could not be changed. Try again!"));
            }

            return RedirectToAction("Campaigns");
        }

        [HttpGet]
        public JsonResult GetDataForReLaunch(int id)
        {
            var campaign = _campaignService.GetCampaignById(id);
            var products = _campaignService.GetProductsOfCampaign(id);
             var result = new RelaunchCampaignsViewModel();
            List<object> prodInfo = new List<object>();
            foreach(var product in products)
            {
                var prodRec = _productService.GetProductById(product.ProductRecord.Id);
                prodInfo.Add(new { Price = product.Price, BaseCostForProduct = prodRec.BaseCost, ProductId = prodRec.Id, BaseCost = product.BaseCost });
            }
            
            var tShirtCostRecord = _tshirtService.GetCost(cultureUsed);

            result.Products = prodInfo.ToArray();
            result.CntBackColor = campaign.CntBackColor;
            result.CntFrontColor = campaign.CntFrontColor;
            result.TShirtCostRecord = tShirtCostRecord;
            result.ProductCountGoal = campaign.ProductCountGoal;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpStatusCodeResult ReLaunchCampaign(int productCountGoal, string campaignProfit, int campaignLength, int minimum, RelaunchProductInfo[] baseCost, int id)
        {       
            var newCampaign = _campaignService.ReLaunchCampiagn(productCountGoal,  campaignProfit,  campaignLength,  minimum,  baseCost,  id);

            CreateImagesForCampaignProducts(newCampaign);

            return new HttpStatusCodeResult(HttpStatusCode.OK);  
        }

        private void CreateImagesForCampaignProducts(CampaignRecord campaign)
        {
            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            var data = serializer.Deserialize<DesignInfo>(campaign.Design);

            foreach (var p in campaign.Products)
            {
                var imageFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                var frontPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_front.png");
                var backPath = Path.Combine(imageFolder, "product_type_" + p.ProductRecord.Id + "_back.png");

                CreateImagesForOtherColor(campaign.Id, p.Id.ToString(), p, data, frontPath, backPath, p.ProductColorRecord.Value);

                if (p.SecondProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.SecondProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.SecondProductColorRecord.Value);
                }
                if (p.ThirdProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.ThirdProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.ThirdProductColorRecord.Value);
                }
                if (p.FourthProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.FourthProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.FourthProductColorRecord.Value);
                }
                if (p.FifthProductColorRecord != null)
                {
                    CreateImagesForOtherColor(campaign.Id, p.Id.ToString() + "_" + p.FifthProductColorRecord.Id.ToString(), p, data, frontPath, backPath, p.FifthProductColorRecord.Value);
                }

                int product = _campaignService.GetProductsOfCampaign(campaign.Id).First().Id;
                string destFolder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaign.Id.ToString(), product.ToString(), "social");
                Directory.CreateDirectory(destFolder);

                var imageSocialFolder = Server.MapPath("/Modules/Teeyoot.Module/Content/images/");
                if (!campaign.BackSideByDefault)
                {
                    var frontSocialPath = Path.Combine(imageSocialFolder, "product_type_" + p.ProductRecord.Id + "_front.png");
                    var imgPath = new Bitmap(frontSocialPath);

                    _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Front);
                }
                else
                {
                    var backSocialPath = Path.Combine(imageSocialFolder, "product_type_" + p.ProductRecord.Id + "_back.png");
                    var imgPath = new Bitmap(backSocialPath);

                    _imageHelper.CreateSocialImg(destFolder, campaign, imgPath, data.Back);
                }
            }
        }

            
        public void CreateImagesForOtherColor(int campaignId, string prodIdAndColor, CampaignProductRecord p, DesignInfo data, string frontPath, string backPath, string color)
        {
            var destForder = Path.Combine(Server.MapPath("/Media/campaigns/"), campaignId.ToString(), prodIdAndColor);

            if (!Directory.Exists(destForder))
            {
                Directory.CreateDirectory(destForder + "/normal");
                Directory.CreateDirectory(destForder + "/big");
            }

            var frontTemplate = new Bitmap(frontPath);
            var backTemplate = new Bitmap(backPath);

            var rgba = ColorTranslator.FromHtml(color);

            var front = BuildProductImage(frontTemplate, _imageHelper.Base64ToBitmap(data.Front), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                p.ProductRecord.ProductImageRecord.PrintableFrontTop, p.ProductRecord.ProductImageRecord.PrintableFrontLeft,
                p.ProductRecord.ProductImageRecord.PrintableFrontWidth, p.ProductRecord.ProductImageRecord.PrintableFrontHeight);
            front.Save(Path.Combine(destForder, "normal", "front.png"));

            var back = BuildProductImage(backTemplate, _imageHelper.Base64ToBitmap(data.Back), rgba, p.ProductRecord.ProductImageRecord.Width, p.ProductRecord.ProductImageRecord.Height,
                p.ProductRecord.ProductImageRecord.PrintableBackTop, p.ProductRecord.ProductImageRecord.PrintableBackLeft,
                p.ProductRecord.ProductImageRecord.PrintableBackWidth, p.ProductRecord.ProductImageRecord.PrintableBackHeight);
            back.Save(Path.Combine(destForder, "normal", "back.png"));

            _imageHelper.ResizeImage(front, 1070, 1274).Save(Path.Combine(destForder, "big", "front.png"));
            _imageHelper.ResizeImage(back, 1070, 1274).Save(Path.Combine(destForder, "big", "back.png"));

            frontTemplate.Dispose();
            backTemplate.Dispose();
            front.Dispose();
            back.Dispose();
        }
        private Bitmap BuildProductImage(Bitmap image, Bitmap design, Color color, int width, int height, int printableAreaTop, int printableAreaLeft, int printableAreaWidth, int printableAreaHeight)
        {
            var background = _imageHelper.CreateBackground(width, height, color);
            image = _imageHelper.ApplyBackground(image, background, width, height);
            return _imageHelper.ApplyDesign(image, design, printableAreaTop, printableAreaLeft, printableAreaWidth, printableAreaHeight, width, height);
        }
    }
}