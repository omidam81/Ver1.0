using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class RelaunchViewModel
    {
        public object[] Products { get; set; }

        public int CntFrontColor { get; set; }

        public int CntBackColor { get; set; }

        public int ProductCountGoal { get; set; }

        public TShirtCostRecord TShirtCostRecord { get; set; }
    }
}