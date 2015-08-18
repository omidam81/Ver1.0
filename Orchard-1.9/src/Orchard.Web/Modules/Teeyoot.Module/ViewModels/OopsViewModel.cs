using System.ComponentModel.DataAnnotations;

namespace Teeyoot.Module.ViewModels
{
    public class OopsViewModel
    {
        public bool RequestAccepted { get; set; }

        public bool InvalidEmail { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}