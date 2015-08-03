using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Teeyoot.Module.Models;
using Teeyoot.WizardSettings.Common;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductSizeViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public SizeMetricDimension WidthDimension { get; set; }

        public double Width { get; set; }

        [Required]
        public SizeMetricDimension LengthDimension { get; set; }

        public double Length { get; set; }
        public SizeMetricDimension SleeveDimension { get; set; }
        public double? Sleeve { get; set; }
        public SelectList SizeMetricDimensions { get; set; }

        [Required]
        public int SelectedSizeCode { get; set; }

        public IEnumerable<SizeCodeRecord> SizeCodes { get; set; }
    }
}