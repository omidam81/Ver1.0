using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class CampaignsViewModel
    {
        public CampaignsViewModel()
        {
            Overviews = new List<CampaignsOverview>();
            Campaigns = new List<CampaignSummary>();
        }

        public IEnumerable<CampaignsOverview> Overviews { get; set; }
        public IEnumerable<CampaignSummary> Campaigns { get; set; }
    }

    public class CampaignSummary
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        public int Goal { get; set; }

        public int Sold { get; set; }

        public double Profit { get; set; }

        public CampaignStatusRecord Status { get; set; }
    }

    public class CampaignsOverview
    {
        public int ProductsOrdered { get; set; }

        public double Profit { get; set; }

        public double ToBePaid { get; set; }

        public OverviewType Type { get; set; }
    }

    public enum OverviewType
    {
        Today = 1,
        Yesterday = 2,
        Active = 4,
        AllTime = 8
    }
}