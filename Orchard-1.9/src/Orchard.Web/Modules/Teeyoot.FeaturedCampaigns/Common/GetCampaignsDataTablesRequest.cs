using DataTables.Mvc;

namespace Teeyoot.FeaturedCampaigns.Common
{
    public class GetCampaignsDataTablesRequest : DefaultDataTablesRequest
    {
        public int? FilterCurrencyId { get; set; }
    }
}