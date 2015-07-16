using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations; 

namespace Teeyoot.Module.Models
{
    public class MessageContentViewModel
    {
        public virtual int Id { get; set; }

        public virtual int CampaignRecord_Id { get; set; }

        [StringLength(50), EmailAddressAttribute, Required(ErrorMessage = "From email can't be blank")]
        public virtual string From { get; set; }

        [StringLength(50), Required(ErrorMessage = "Subject can't be blank")]
        public virtual string Subject { get; set; }
       [StringLength(50), EmailAddressAttribute, Required(ErrorMessage = "Recipient can't be blank")]
        public virtual string Email { get; set; }
        [StringLength(50), Required(ErrorMessage = "Message can't be blank")]
        public virtual string Content { get; set; }

        
    }
}