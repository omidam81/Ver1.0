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

        public string MYCurrencyCode { get; set; }

        public string IDCurrencyCode { get; set; }

        public string SGCurrencyCode { get; set; }
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

        public int Minimum { get; set; }

        public double Profit { get; set; }

        public string SummaryCurrency { get; set; }

        public string Alias { get; set; }

        public CampaignStatusRecord Status { get; set; }

        public bool IsActive { get; set; }

        public bool IsArchived { get; set; }

        public bool ShowBack { get; set; }

        public bool IsPrivate { get; set; }

        public int CountRequests { get; set; }
    }

    public class CampaignsOverview
    {
        public int ProductsOrdered { get; set; }

        public double MYProfit { get; set; }

        public double SGProfit { get; set; }

        public double IDProfit { get; set; }

        public double MYToBePaid { get; set; }

        public double SGToBePaid { get; set; }

        public double IDToBePaid { get; set; }

        public double MYToBeAllPaid { get; set; }

        public OverviewType Type { get; set; }
    }
}