using System.ComponentModel.DataAnnotations;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductStyleViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}