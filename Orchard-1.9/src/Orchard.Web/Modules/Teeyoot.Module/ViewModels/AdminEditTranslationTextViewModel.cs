using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.ViewModels
{
    public class AdminEditTranslationTextViewModel
    {
        public string ChangeText { get; set; }
        public string ReplaceText { get; set; }
        public string FilePath { get; set; }
        public int ActionCountry { get; set; }
        public int ActionCulture { get; set; }
        public string Search { get; set; }
    }
}