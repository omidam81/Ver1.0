namespace Teeyoot.Account.ViewModels
{
    public class AccountIndexViewModel
    {
        public bool RegistrationValidationIssueOccurred { get; set; }
        public string RegistrationValidationSummary { get; set; }
        public bool LoggingOnValidationIssueOccured { get; set; }
        public string LoggingOnValidationSummary { get; set; }
    }
}