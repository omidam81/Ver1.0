namespace Teeyoot.FeaturedCampaigns.ViewModels
{
    public class CampaignItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string CreatedDate { get; set; }
        public string IsActive { get; set; }
        public string Last24HoursSold { get; set; }
        public int Sold { get; set; }
        public int Minimum { get; set; }
        public int Goal { get; set; }
        public string Profit { get; set; }
        public string Status { get; set; }
        public string EndDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Currency { get; set; }
    }
}