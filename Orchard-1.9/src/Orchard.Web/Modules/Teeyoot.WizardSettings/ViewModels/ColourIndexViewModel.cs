using System.Collections.Generic;
using Teeyoot.WizardSettings.Common;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ColourIndexViewModel
    {
        public ChooseColourFor? ChooseColourFor { get; set; }
        public IEnumerable<dynamic> Colors { get; set; }
        public dynamic Pager { get; set; }

        public ColourIndexViewModel(ChooseColourFor? chooseColourFor)
        {
            ChooseColourFor = chooseColourFor;
        }
    }
}