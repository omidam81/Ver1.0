namespace Teeyoot.Account.ViewModels
{
    public class RecoverViewModel
    {
        public bool RecoverEmailSent { get; set; }
        public bool RecoverFailed { get; set; }
        public string RecoverIssueSummary { get; set; }
        public string Email { get; set; }
    }
}