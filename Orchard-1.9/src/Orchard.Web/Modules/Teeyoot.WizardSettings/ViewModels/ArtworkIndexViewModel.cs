using System.Collections.Generic;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ArtworkIndexViewModel
    {
        public dynamic Pager { get; set; }
        public IEnumerable<ArtworkItemViewModel> Artworks { get; set; }
    }
}