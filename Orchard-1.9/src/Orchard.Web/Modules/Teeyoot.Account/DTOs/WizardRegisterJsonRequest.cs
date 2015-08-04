namespace Teeyoot.Account.DTOs
{
    public class WizardRegisterJsonRequest
    {
        public int CampaignId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}