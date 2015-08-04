using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.Messaging.ViewModels
{
    public class AdminMessagesViewModel
    {
       
        public dynamic[] Messages { get; set; }

        public dynamic Pager { get; set; }
    
        public int Id { get; set; }

        public string Text { get; set; }

        public string Sender { get; set; }

        public DateTime SendDate { get; set; }

        public int UserId { get; set; }

        public string Subject { get; set; }


    }
}