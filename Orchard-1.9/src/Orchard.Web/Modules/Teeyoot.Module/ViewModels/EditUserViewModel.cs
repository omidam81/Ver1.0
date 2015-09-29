using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Teeyoot.Module.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        public string Email { get; set; }
        /*
        [PasswordPropertyText]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [PasswordPropertyText]
        public string ConfirmPassword { get; set; }
         */
    }
}