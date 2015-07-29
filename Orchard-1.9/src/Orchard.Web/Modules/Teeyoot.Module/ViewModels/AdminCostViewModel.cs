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
        public string FirstScreenCost { get; set; }

        [Required]
        public string AdditionalScreenCosts { get; set; }

        [Required]
        public string InkCost { get; set; }

        [Required]
        public int PrintsPerLitre { get; set; }

        [Required]
        public string LabourCost { get; set; }

        [Required]
        public int LabourTimePerColourPerPrint { get; set; }

        [Required]
        public int LabourTimePerSidePrintedPerPrint { get; set; }

        [Required]
        public string CostOfMaterial { get; set; }

        [Required]
        public string PercentageMarkUpRequired { get; set; }

        [Required]
        public string DTGPrintPrice { get; set; }
    }
}