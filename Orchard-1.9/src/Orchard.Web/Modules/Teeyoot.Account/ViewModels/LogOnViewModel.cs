namespace Teeyoot.Account.ViewModels
{
    public class LogOnViewModel
    {
        public LogOnViewModel()
        {
        }

        public LogOnViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}