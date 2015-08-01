using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class CampaignIndexViewModel
    {
        public virtual CampaignRecord Campaign { get; set; }

        public virtual string InfoMessage { get; set; }


        
    }
}