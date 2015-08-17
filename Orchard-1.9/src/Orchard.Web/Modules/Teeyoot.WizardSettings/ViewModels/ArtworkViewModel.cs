﻿using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ArtworkViewModel
    {
        public string CurrentName { get; set; }

        [Required]
        public string Name { get; set; }

        public HttpPostedFileBase ArtworkSvgImage { get; set; }
        public HttpPostedFileBase ArtworkPngImage { get; set; }
    }
}