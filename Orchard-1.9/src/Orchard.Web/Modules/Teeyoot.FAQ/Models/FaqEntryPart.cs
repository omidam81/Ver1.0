using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Teeyoot.FAQ.Models
{
    public class FaqEntryPart : ContentPart<FaqEntryPartRecord>
    {
        public string Question
        {
            get { return Retrieve(p => p.Question); }
            set { Store(p => p.Question, value); }
        }

        public string Answer
        {
            get { return Retrieve(p => p.Answer); }
            set { Store(p => p.Answer, value); }
        }

        public LanguageRecord Language
        {
            get { return Record.LanguageRecord; }
            set { Record.LanguageRecord = value; }
        }

        public FaqSectionRecord Section
        {
            get { return Record.FaqSectionRecord; }
            set { Record.FaqSectionRecord = value; }
        }

        public string LanguageCode { get; set; }

        public int SectionId { get; set; }

        public BodyPart Body
        {
            get { return this.As<BodyPart>(); }
        }

        [HiddenInput(DisplayValue = false)]
        public IEnumerable<LanguageRecord> AvailableLanguages { get; set; }

        [HiddenInput(DisplayValue = false)]
        public IEnumerable<FaqSectionRecord> AvailableSections { get; set; }
    }
}