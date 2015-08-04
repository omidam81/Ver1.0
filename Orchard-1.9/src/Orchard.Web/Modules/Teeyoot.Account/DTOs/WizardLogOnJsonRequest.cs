namespace Teeyoot.Account.DTOs
{
    public class WizardLogOnJsonRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}