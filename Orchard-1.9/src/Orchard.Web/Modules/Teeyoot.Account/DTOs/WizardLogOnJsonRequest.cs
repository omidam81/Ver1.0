namespace Teeyoot.Account.DTOs
{
    public class WizardLogOnJsonRequest
    {
        public int CampaignId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}