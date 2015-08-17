using System.Collections.Generic;
using Teeyoot.Module.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ArtworkIndexViewModel
    {
        public dynamic Pager { get; set; }
        public string ArtworksImagesRelativePath { get; set; }
        public IEnumerable<ArtRecord> Arts { get; set; }
    }
}