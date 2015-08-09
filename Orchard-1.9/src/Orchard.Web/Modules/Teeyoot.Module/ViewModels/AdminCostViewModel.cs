using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

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
        public string PercentageMarkUpRequired { get; set; }

        [Required]
        public string DTGPrintPrice { get; set; }

        public CampaignRecord Campaign { get; set; }

        public List<CampaignProductRecord> Products { get; set; }

        public int SalesGoal { get; set; }

        public string FacebookClientId { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleApiKey { get; set; }
    }
}