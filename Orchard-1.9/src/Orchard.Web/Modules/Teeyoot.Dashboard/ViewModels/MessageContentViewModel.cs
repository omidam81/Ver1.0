using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; 

namespace Teeyoot.Module.Dashboard.ViewModels
{
    public class MessageContentViewModel
    {
        public int Id { get; set; }

        [StringLength(50), EmailAddressAttribute, Required(ErrorMessage = "From email can't be blank")]
        public string From { get; set; }

        [StringLength(50), Required(ErrorMessage = "Subject can't be blank")]
        public string Subject { get; set; }
        [StringLength(50), EmailAddressAttribute, Required(ErrorMessage = "Recipient can't be blank")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Message can't be blank")]
        public string Content { get; set; }
    }
}