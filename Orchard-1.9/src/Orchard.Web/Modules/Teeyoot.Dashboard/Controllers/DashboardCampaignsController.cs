using System.Web.Mvc;
using System.Linq;
using Teeyoot.Dashboard.ViewModels;
using Teeyoot.Module.Models;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Common.ExtentionMethods;
using System;

namespace Teeyoot.Dashboard.Controllers
{
    public partial class DashboardController : Controller
    {
        public ActionResult Campaigns()
        {
            var model = new CampaignsViewModel();
            model.Currency = "RM"; //TODO: eugene: implement currency
            var user = _wca.GetContext().CurrentUser;
            var teeyootUser = user.ContentItem.Get(typeof(TeeyootUserPart));
            var campaignsQuery = _campaignService.GetCampaignsOfUser(teeyootUser != null ? teeyootUser.Id : 0);
            var productsOrderedQuery = _orderService.GetProductsOrderedOfCampaigns(campaignsQuery.Select(c => c.Id).ToArray());

            FillOverviews(model, productsOrderedQuery, campaignsQuery);

            return View(model);
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
                            //.Where(p => p.OrderRecord.Paid.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
                //,
                //ToBePaid = productsOrderedQuery
                //            .FilterByType(OverviewType.Today)
                //            .Where(p => !p.OrderRecord.Paid.HasValue)
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
                            //.Where(p => p.OrderRecord.Paid.HasValue)
                            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                            .Sum(entry => (int?)entry.Profit) ?? 0
                //,
                //ToBePaid = productsOrderedQuery
                //            .FilterByType(OverviewType.Yesterday)
                //            .Where(p => !p.OrderRecord.Paid.HasValue)
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
                            .Where(p => !p.OrderRecord.Paid.HasValue)
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
                //            .Where(p => !p.OrderRecord.Paid.HasValue)
                //            .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                //            .Sum(entry => entry.Profit)
            });
        }
    }
}