using System.Web;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ArtworkViewModel
    {
        public string Name { get; set; }
        public HttpPostedFileBase ArtworkImage { get; set; }
    }
}