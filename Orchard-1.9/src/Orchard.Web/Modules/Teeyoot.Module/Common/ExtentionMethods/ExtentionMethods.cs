using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Common.ExtentionMethods
{
    public static class ExtentionMethods
    {
        public static IQueryable<LinkOrderCampaignProductRecord> FilterByType(this IQueryable<LinkOrderCampaignProductRecord> query, OverviewType type, IQueryable<CampaignRecord> campaignsQuery = null)
        {
            var today = DateTime.UtcNow.Date;
            switch (type)
            {                   
                case OverviewType.Active:
                    return query
                            .Where(p => campaignsQuery
                                .Where(c => c.IsActive)
                                .Select(c => c.Id).Contains(p.CampaignProductRecord.CampaignRecord_Id));
                case OverviewType.Today:
                    var nextDay = today.AddDays(1);
                    return query
                            .Where(p => p.OrderRecord.Created.Date >= today && p.OrderRecord.Created.Date < nextDay);
                case OverviewType.Yesterday:
                    var yesterday = today.AddDays(-1);
                    return query
                            .Where(p => p.OrderRecord.Created.Date >= yesterday && p.OrderRecord.Created.Date < today);
                default:
                    return query;
            }
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }
}