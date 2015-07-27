namespace Teeyoot.Account.ViewModels
{
    public class ResetPasswordViewModel
    {
        public bool ResetPasswordIssueOccurred { get; set; }
        public string ResetPasswordValidationSummary { get; set; }
        public string Nonce { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}