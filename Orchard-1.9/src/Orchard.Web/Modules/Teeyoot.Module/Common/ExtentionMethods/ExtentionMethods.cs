using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Common.ExtentionMethods
{
    public static class ExtentionMethods
    {
        public static IQueryable<LinkOrderCampaignProductRecord> FilterByType(this IQueryable<LinkOrderCampaignProductRecord> query, OverviewType type, IQueryable<CampaignRecord> campaignsQuery = null)
        {
            switch (type)
            {
                case OverviewType.Active:
                    return query
                            .Where(p => campaignsQuery
                                .Where(c => c.CampaignStatusRecord.Name == CampaignStatus.Active.ToString())
                                .Select(c => c.Id).Contains(p.CampaignProductRecord.CampaignRecord_Id));
                case OverviewType.Today:
                    return query
                            .Where(p => p.OrderRecord.Created.ToLocalTime() == DateTime.Now);
                case OverviewType.Yesterday:
                    return query
                            .Where(p => p.OrderRecord.Created.ToLocalTime() == DateTime.Now.AddDays(-1));
                default:
                    return query;
            }
        }
    }
}