using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class AdminCostViewModel
    {
        public float FirstScreenCost { get; set; }

        public float AdditionalScreenCosts { get; set; }

        public float InkCost { get; set; }

        public int PrintsPerLitre { get; set; }

        public float LabourCost { get; set; }

        public int LabourTimePerColourPerPrint { get; set; }

        public int LabourTimePerSidePrintedPerPrint { get; set; }

        public float CostOfMaterial { get; set; }

        public float PercentageMarkUpRequired { get; set; }

        public float DTGPrintPrice { get; set; } 
    }
}