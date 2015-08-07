using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class ReservationCompleteViewModel
    {
        public ReservationCompleteViewModel()
        {

        }

        public dynamic[] Campaigns { get; set; }

        public string Message { get; set; }
    }
}