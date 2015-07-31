using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductIndexViewModel
    {
        public IEnumerable<ProductRecord> Products { get; set; }
        public dynamic Pager { get; set; }

        public ProductIndexViewModel()
        {
        }

        public ProductIndexViewModel(
            IEnumerable<ProductRecord> products,
            dynamic pager)
        {
            Products = products;
            Pager = pager;
        }
    }
}