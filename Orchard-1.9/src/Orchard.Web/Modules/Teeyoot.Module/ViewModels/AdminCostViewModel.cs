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
        public decimal FirstScreenCost { get; set; }

        [Required]
        public decimal AdditionalScreenCosts { get; set; }

        [Required]
        public decimal InkCost { get; set; }

        [Required]
        public int PrintsPerLitre { get; set; }

        [Required]
        public decimal LabourCost { get; set; }

        [Required]
        public int LabourTimePerColourPerPrint { get; set; }

        [Required]
        public int LabourTimePerSidePrintedPerPrint { get; set; }

        [Required]
        public decimal CostOfMaterial { get; set; }

        [Required]
        public decimal PercentageMarkUpRequired { get; set; }

        [Required]
        public decimal DTGPrintPrice { get; set; }
    }
}