using Orchard.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Orchard.Localization.Records;


namespace Teeyoot.Module.ViewModels
{
    public class CountryViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public IEnumerable<SelectedCultureItem> Cultures { get; set; }
        public IEnumerable<int> SelectedCultures { get; set; }



        public CountryViewModel(IEnumerable<SelectedCultureItem> cultures)
        {
            Cultures = cultures;
        }

        public CountryViewModel()
        {
        }

    }
}

