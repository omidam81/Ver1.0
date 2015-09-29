using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Dashboard.ViewModels
{
    public class CampaignIndexViewModel
    {
        public virtual CampaignRecord Campaign { get; set; }

        public virtual string PromoId { get; set; }

        public int CntRequests { get; set; }

        public virtual double PromoSize { get; set; }

        public virtual string PromoType { get; set; }

        public virtual string FBDescription { get; set; }

    }
}