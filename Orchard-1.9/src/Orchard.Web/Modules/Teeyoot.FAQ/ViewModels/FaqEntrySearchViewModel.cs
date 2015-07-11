using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.FAQ.ViewModels
{
    public class FaqEntrySearchViewModel
    {
        public string SearchString { get; set; }

        public string LanguageCode { get; set; }

        public int SectionId { get; set; }
    }
}