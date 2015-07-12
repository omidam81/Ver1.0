using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ.ViewModels
{
    public class ViewSectionViewModel
    {
        public FaqSectionRecord Section { get; set; }
        public FaqEntryPart[] Topics { get; set; }
    }
}