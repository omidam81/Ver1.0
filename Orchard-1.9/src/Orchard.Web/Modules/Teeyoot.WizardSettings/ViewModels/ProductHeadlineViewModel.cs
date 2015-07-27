using System.ComponentModel.DataAnnotations;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductHeadlineViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}