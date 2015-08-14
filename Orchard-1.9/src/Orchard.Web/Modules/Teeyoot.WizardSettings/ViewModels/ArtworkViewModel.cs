using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ArtworkViewModel
    {
        [Required]
        public string Name { get; set; }

        public HttpPostedFileBase ArtworkImage { get; set; }
    }
}