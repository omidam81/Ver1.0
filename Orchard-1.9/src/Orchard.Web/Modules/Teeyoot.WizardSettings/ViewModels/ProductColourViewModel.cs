﻿using System.ComponentModel.DataAnnotations;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductColourViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}