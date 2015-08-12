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

        public CampaignRecord Campaign { get; set; }


    }
}