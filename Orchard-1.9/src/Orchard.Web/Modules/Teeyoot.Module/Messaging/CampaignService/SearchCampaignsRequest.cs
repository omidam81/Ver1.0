namespace Teeyoot.Module.Messaging.CampaignService
{
    public class SearchCampaignsRequest
    {
        public string Filter { get; set; }
        public string Tag { get; set; }
        public string Culture { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}