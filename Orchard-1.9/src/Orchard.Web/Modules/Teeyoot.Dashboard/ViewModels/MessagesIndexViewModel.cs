using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Teeyoot.Module.Models; 

namespace Teeyoot.Module.Dashboard.ViewModels
{
    public class MessagesIndexViewModel
    {
        public int ThisWeekSend { get; set; }
        
        public string LastSend { get; set; }

        public MessagesCampaignViewModel Campaign { get; set; }
    }

    public class MessagesCampaignViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Sold { get; set; }

        public int FirstProductId { get; set; }

        public bool BackByDefault { get; set; }
    }
}