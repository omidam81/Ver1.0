using System;

namespace Teeyoot.FeaturedCampaigns.Models
{
    public class CampaignItem
    {
        public double Profit { get; set; }
        public int Last24HoursSold { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public int Goal { get; set; }
        public int Sold { get; set; }
        public bool IsApproved { get; set; }
        public DateTime EndDate { get; set; }
        public string Alias { get; set; }
        public bool IsActive { get; set; }
        public int Minimum { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Currency { get; set; }
    }
}