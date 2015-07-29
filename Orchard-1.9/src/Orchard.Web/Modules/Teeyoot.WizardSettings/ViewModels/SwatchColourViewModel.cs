using System.ComponentModel.DataAnnotations;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class SwatchColourViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public bool InStock { get; set; }
    }
}