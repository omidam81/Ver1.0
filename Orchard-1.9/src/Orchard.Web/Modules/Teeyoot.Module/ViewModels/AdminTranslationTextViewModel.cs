using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using RM.Localization.Models;
using Orchard.Localization.Records;

namespace Teeyoot.Module.ViewModels
{
    public class AdminTranslationTextViewModel
    {
        public string SearchString { get; set; }
        public bool NotFoundResult { get; set; }
        public IList<CountryRecord> ActionCountry { get; set; }
        public IList<CultureRecord> ActionCulture { get; set; }
        public int ActionCountryId { get; set; }
        public int ActionCultureId { get; set; }
        public bool NotFoundCulture { get; set; }
        public List<string> SearchResult { get; set; }
        public List<string> SearchResultReplace { get; set; }
        public List<string> SearchResultFilePath { get; set; }
        public int ChangeText { get; set; }
    }
}