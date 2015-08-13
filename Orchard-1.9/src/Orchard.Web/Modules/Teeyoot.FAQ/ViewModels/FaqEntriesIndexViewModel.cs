using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.ViewModels
{
    public class FaqEntriesIndexViewModel
    {
        public dynamic[] FaqEntries { get; set; }
        public dynamic Pager { get; set; }
        public FaqEntrySearchViewModel Search { get; set; }

        public FaqSectionRecord[] Sections { get; set; }
        public LanguageViewModel[] Languages { get; set; }
 
        public FaqEntriesIndexViewModel() {
            Search = new FaqEntrySearchViewModel();
        }

        public FaqEntriesIndexViewModel(IEnumerable<dynamic> entries, IEnumerable<FaqSectionRecord> sections, IEnumerable<LanguageViewModel> languages, FaqEntrySearchViewModel search, dynamic pager)
        {
            Sections = sections.ToArray();
            Languages = languages.ToArray();
            FaqEntries = entries.ToArray();
            Search = search;
            Pager = pager;
        }
    }

    public class LanguageViewModel
    {
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
    }
}