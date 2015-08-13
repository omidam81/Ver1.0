namespace Teeyoot.Account.ViewModels
{
    public class CreateAccountViewModel
    {
        public CreateAccountViewModel()
        {
        }

        public CreateAccountViewModel(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string ConfirmPassword { get; set; }
    }
}