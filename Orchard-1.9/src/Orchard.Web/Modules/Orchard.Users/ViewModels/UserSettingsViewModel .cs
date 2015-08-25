using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Users.Models;

namespace Orchard.Users.ViewModels {
    public class UserSettingsViewModel  {

        public int Id { get; set; }
        
        public string PublicName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string Suit { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public string CurrentEmail { get; set; }

        [EmailAddress]
        public string NewEmailAddress { get; set; }

        [EmailAddress]
        public string ConfirmNewEmailAddress { get; set; }
        
        public string CurrentPassword { get; set; }
       
        public string NewPassword { get; set; }
      
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }

        public string InfoMessage { get; set; }

    }
}