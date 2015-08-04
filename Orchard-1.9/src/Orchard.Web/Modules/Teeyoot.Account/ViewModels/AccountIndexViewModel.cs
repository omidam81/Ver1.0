namespace Teeyoot.Account.ViewModels
{
    public class AccountIndexViewModel
    {
        public AccountIndexViewModel()
        {
        }

        public AccountIndexViewModel(CreateAccountViewModel createAccountViewModel, LogOnViewModel logOnViewModel)
        {
            CreateAccountViewModel = createAccountViewModel;
            LogOnViewModel = logOnViewModel;
        }

        public CreateAccountViewModel CreateAccountViewModel { get; set; }
        public LogOnViewModel LogOnViewModel { get; set; }

        public bool RegistrationValidationIssueOccurred { get; set; }
        public string RegistrationValidationSummary { get; set; }
        public bool LoggingOnValidationIssueOccurred { get; set; }
        public string LoggingOnValidationSummary { get; set; }
        public bool PasswordHasBeenUpdated { get; set; }
    }
}