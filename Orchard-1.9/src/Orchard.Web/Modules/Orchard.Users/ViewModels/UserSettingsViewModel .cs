using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Users.Models;

namespace Orchard.Users.ViewModels {
    public class UserSettingsViewModel  {
       
        public string PublicName { get; set; }

        public string PhoneNumber { get; set; }

        public string Street { get; set; }

        public string Suit { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        [DataType(DataType.EmailAddress)]
        public string NewEmailAddress { get; set; }

        [DataType(DataType.EmailAddress)]
        public string ConfirmNewEmailAddress { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string ShowNumberSold { get; set; }
    }
}