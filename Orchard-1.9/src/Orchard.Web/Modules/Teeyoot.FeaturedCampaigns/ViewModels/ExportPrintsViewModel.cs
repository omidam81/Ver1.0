using System.Collections.Generic;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class ExportPrintsViewModel
    {
        public dynamic[] Campaigns { get; set; }
        public dynamic Pager { get; set; }
        public int StartedIndex { get; set; }
        public string SearchString { get; set; }
        public int NotApprovedTotal { get; set; }
        public IEnumerable<CurrencyItemViewModel> Currencies { get; set; }
        public IEnumerable<string> CampaignStatuses { get; set; }
    }
}