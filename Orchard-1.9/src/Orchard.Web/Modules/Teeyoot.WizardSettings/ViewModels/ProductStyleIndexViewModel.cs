using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductStyleIndexViewModel
    {
        public IEnumerable<ProductGroupRecord> ProductStyles { get; set; }
        public dynamic Pager { get; set; }

        public ProductStyleIndexViewModel()
        {
        }

        public ProductStyleIndexViewModel(
            IEnumerable<ProductGroupRecord> productStyles,
            dynamic pager)
        {
            ProductStyles = productStyles;
            Pager = pager;
        }
    }
}