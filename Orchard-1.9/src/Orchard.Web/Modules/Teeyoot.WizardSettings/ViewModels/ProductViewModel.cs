﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel() : this(null)
        {
        }

        public ProductViewModel(int? productId)
        {
            Id = productId;

            ProductColours = new List<ProductColourItemViewModel>();
            SelectedProductColours = new List<int>();
        }

        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IEnumerable<ProductColourItemViewModel> ProductColours { get; set; }
        public IEnumerable<int> SelectedProductColours { get; set; }
    }
}