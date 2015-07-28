using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class FontViewModel
    {
        public int Id { get; set; }

        public string Family { get; set; }
        
        public string FileName { get; set; }

        public string Tags { get; set; }

        public int? Priority { get; set; }

        public string TtfFile { get; set; }

        public string WoffFile { get; set; }

        public string Thumbnail { get; set; }



    }
}