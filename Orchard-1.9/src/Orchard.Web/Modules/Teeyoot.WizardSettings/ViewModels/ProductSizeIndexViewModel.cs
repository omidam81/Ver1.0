using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductSizeIndexViewModel
    {
        public IEnumerable<ProductSizeRecord> ProductSizes { get; set; }
        public dynamic Pager { get; set; }

        public ProductSizeIndexViewModel()
        {
        }

        public ProductSizeIndexViewModel(IEnumerable<ProductSizeRecord> productSizes, dynamic pager)
        {
            ProductSizes = productSizes;
            Pager = pager;
        }
    }
}