using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Teeyoot.Module.Models; 

namespace Teeyoot.Module.Dashboard.ViewModels
{
    public class MessageContentViewModel
    {
        public string CampaignTitle { get; set; }
        
        public int CampaignId { get; set; }

        public int ProductId { get; set; }

        [StringLength(50), EmailAddressAttribute, Required(ErrorMessage = "From email can't be blank")]
        public string From { get; set; }

        [StringLength(50), Required(ErrorMessage = "Subject can't be blank")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message can't be blank")]
        public string Content { get; set; }

        public string Status { get; set; }


    }
}