using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class ExportPrintsViewModel
    {
        public dynamic[] Campaigns { get; set; }

        public dynamic Pager { get; set; }

        public int StartedIndex { get; set; }

        public string SearchString { get; set; }
    }
}