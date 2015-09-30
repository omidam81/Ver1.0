using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Teeyoot.Module.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [PasswordPropertyText]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [PasswordPropertyText]
        public string ConfirmPassword { get; set; }

        public bool IsTeeyootUser { get; set; }

        public bool IsUserCurrencyEditable { get; set; }

        public IEnumerable<CurrencyItemViewModel> Currencies { get; set; }

        public int? Currency { get; set; }
    }
}