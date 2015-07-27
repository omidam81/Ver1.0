using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductHeadlineIndexViewModel
    {
        public IEnumerable<ProductHeadlineRecord> ProductHeadlines { get; set; }
        public dynamic Pager { get; set; }

        public ProductHeadlineIndexViewModel()
        {
        }

        public ProductHeadlineIndexViewModel(
            IEnumerable<ProductHeadlineRecord> productHeadlines,
            dynamic pager)
        {
            ProductHeadlines = productHeadlines;
            Pager = pager;
        }
    }
}