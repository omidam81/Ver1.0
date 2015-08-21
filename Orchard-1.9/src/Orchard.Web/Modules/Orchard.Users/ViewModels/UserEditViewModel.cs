using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.Users.Models;

namespace Orchard.Users.ViewModels {
    public class UserEditViewModel  {
        [Required]
        public string UserName {
            get { return User.As<UserPart>().UserName; }
            set { User.As<UserPart>().UserName = value; }
        }

        [Required]
        public string Email {
            get { return User.As<UserPart>().Email; }
            set { User.As<UserPart>().Email = value; }
        }

        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public IContent User { get; set; }
    }
}