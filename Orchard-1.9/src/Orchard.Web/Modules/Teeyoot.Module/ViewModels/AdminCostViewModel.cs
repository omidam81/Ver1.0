using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class AdminCostViewModel
    {
        [Required]
        public float FirstScreenCost { get; set; }

        [Required]
        public float AdditionalScreenCosts { get; set; }

        [Required]
        public float InkCost { get; set; }

        [Required]
        public int PrintsPerLitre { get; set; }

        [Required]
        public float LabourCost { get; set; }

        [Required]
        public int LabourTimePerColourPerPrint { get; set; }

        [Required]
        public int LabourTimePerSidePrintedPerPrint { get; set; }

        [Required]
        public float CostOfMaterial { get; set; }

        [Required]
        public float PercentageMarkUpRequired { get; set; }

        [Required]
        public float DTGPrintPrice { get; set; } 
    }
}