﻿using System;
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
        public LanguageRecord[] Languages { get; set; }
 
        public FaqEntriesIndexViewModel() {
            Search = new FaqEntrySearchViewModel();
        }

        public FaqEntriesIndexViewModel(IEnumerable<dynamic> entries, IEnumerable<FaqSectionRecord> sections, IEnumerable<LanguageRecord> languages, FaqEntrySearchViewModel search, dynamic pager)
        {
            Sections = sections.ToArray();
            Languages = languages.ToArray();
            FaqEntries = entries.ToArray();
            Search = search;
            Pager = pager;
        }
    }
}