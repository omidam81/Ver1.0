using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class CampaignsViewModel
    {
        public CampaignsViewModel()
        {
            Overviews = new List<CampaignsOverview>();
        }
        public IList<CampaignsOverview> Overviews { get; set; }

        public CampaignSummary[] Campaigns { get; set; }

        public string Currency { get; set; }
    }

    public class CampaignSummary
    {
        public int Id { get; set; }

        public int FirstProductId { get; set; }

        public string Name { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime StartDate { get; set; }

        public int Goal { get; set; }

        public int Sold { get; set; }

        public double Profit { get; set; }

        public string Alias { get; set; }

        public CampaignStatusRecord Status { get; set; }

        public bool IsActive { get; set; }

        public bool ShowBack { get; set; }

        public bool IsPrivate { get; set; }
    }

    public class CampaignsOverview
    {
        public int ProductsOrdered { get; set; }

        public double Profit { get; set; }

        public double ToBePaid { get; set; }

        public OverviewType Type { get; set; }
    }
}