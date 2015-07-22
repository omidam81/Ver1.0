using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.ViewModels
{
    public class SearchViewModel
    {
        public bool NotResult { get; set; }

        public string Filter { get; set; }

        public List<CampaignRecord> CampList { get; set; }

        public int NewRow { get; set; }

        public bool NotFoundCategories { get; set; }

        public List<CampaignCategoriesRecord> CampCategList { get; set; }

        public float[] Price { get; set; }
    }
}