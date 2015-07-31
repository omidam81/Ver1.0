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

        public ActionResult Campaigns()
        {
            var model = new CampaignsViewModel();
            model.Currency = "RM"; //TODO: eugene: implement currency
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
            var campaignSummaries = new List<CampaignSummary>();
            var campaigns = campaignsQuery.OrderBy(c => c.StartDate).ToList();

            foreach (var c in campaigns)
            {
                campaignSummaries.Add(new CampaignSummary
                {
                    Alias = c.Alias,
                    EndDate = c.EndDate,
                    Goal = c.ProductCountGoal,
                    Id = c.Id,
                    Name = c.Title,
                    Sold = c.ProductCountSold,
                    StartDate = c.StartDate,
                    Status = c.CampaignStatusRecord,
                    ShowBack = c.BackSideByDefault,
                    FirstProductId = c.Products[0].Id,
                    Profit = _orderService.GetProductsOrderedOfCampaign(c.Id)
                                        .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                                        .Sum(entry => (int?)entry.Profit) ?? 0
                });
            }
            model.Campaigns = campaignSummaries;
        }

        private void FillOverviews(CampaignsViewModel model, IQueryable<LinkOrderCampaignProductRecord> productsOrderedQuery, IQueryable<CampaignRecord> campaignsQuery)
        {           
            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.Today,
                ProductsOrdered = productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                            .Sum(p => (int?)p.Count) ?? 0,
                Profit = productsOrderedQuery
                            .FilterByType(OverviewType.Today)
                            .Where(p => p.OrderRecord.Reserved.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
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
                            .Sum(p => (int?)p.Count) ?? 0,
                Profit = productsOrderedQuery
                            .FilterByType(OverviewType.Yesterday)
                            .Where(p => p.OrderRecord.Reserved.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
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
                            .Sum(p => (int?)p.Count) ?? 0,
                Profit = productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => p.OrderRecord.Paid.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0,
                ToBePaid = productsOrderedQuery
                            .FilterByType(OverviewType.Active, campaignsQuery)
                            .Where(p => p.OrderRecord.Reserved.HasValue && !p.OrderRecord.Paid.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
            });

            model.Overviews.Add(new CampaignsOverview
            {
                Type = OverviewType.AllTime,
                ProductsOrdered = productsOrderedQuery
                            .Sum(p => (int?)p.Count) ?? 0,
                Profit = productsOrderedQuery
                            .Where(p => p.OrderRecord.Paid.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
                //,
                //ToBePaid = productsOrderedQuery
                //            .Where(p => !p.OrderRecord.Reserved.HasValue)
                //            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                //            .Sum(entry => entry.Profit)
            });
        }

        [Themed]
        public ActionResult EditCampaign(int id)
        {
            CampaignRecord camp = _campaignService.GetCampaignById(id);

            return View("EditCampaign");
        }
    }
}