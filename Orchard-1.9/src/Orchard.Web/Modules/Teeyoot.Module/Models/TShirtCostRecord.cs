using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class TShirtCostRecord
    {
        public virtual int Id { get; set; }
        public virtual float FirstScreenCost { get; set; }

        public virtual float AdditionalScreenCosts { get; set; }

        public virtual float InkCost { get; set; }

        public virtual int PrintsPerLitre { get; set; }

        public virtual float LabourCost { get; set; }

        public virtual int LabourTimePerColourPerPrint { get; set; }

        public virtual int LabourTimePerSidePrintedPerPrint { get; set; }

        public virtual float PercentageMarkUpRequired { get; set; }

        public virtual float DTGPrintPrice { get; set; }
    }
}