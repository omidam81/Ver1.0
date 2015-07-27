using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductColourListViewModel
    {
        public IEnumerable<ProductColorRecord> ProductColors { get; set; }
        public dynamic Pager { get; set; }

        public ProductColourListViewModel()
        {
        }

        public ProductColourListViewModel(
            IEnumerable<ProductColorRecord> productColors,
            dynamic pager)
        {
            ProductColors = productColors;
            Pager = pager;
        }
    }
}