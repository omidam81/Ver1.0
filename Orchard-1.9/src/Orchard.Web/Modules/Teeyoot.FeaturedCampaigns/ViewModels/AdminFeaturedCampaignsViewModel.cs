﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Orchard.Data;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class AdminFeaturedCampaignsViewModel
    {
        public FeaturedCampaignViewModel[] Campaigns { get; set; }

        public dynamic Pager { get; set; }

        public int StartedIndex { get; set; }

        public int NotApprovedTotal { get; set; }

        public IRepository<CurrencyRecord> Currencies { get; set; }

        public int FilterCurrencyId { get; set; }

    }

    public class FeaturedCampaignViewModel
    {
        public CampaignViewModel Campaign { get; set; }

        public int Last24HoursSold { get; set; }
    }

    public class CampaignViewModel
    {
        public int Id { get; set; }

        public bool IsFeatured { get; set; }

        public int Sold { get; set; }

        public int Goal { get; set; }

        public int Minimum { get; set; }

        public string Title { get; set; }

        public bool IsActive { get; set; }

        public bool IsApproved { get; set; }

        public bool Rejected { get; set; }

        public string Alias { get; set; }

        public DateTime CreatedDate { get; set; }

        public CurrencyRecord Currency { get; set; }

        public int? FilterCurrencyId { get; set; }
    }
}