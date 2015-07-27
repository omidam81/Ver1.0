using Teeyoot.WizardSettings.Models;

namespace Teeyoot.WizardSettings.ViewModels
{
    public class ColourIndexViewModel
    {
        public ColourIndexViewModel(ChooseColourFor? chooseColourFor)
        {
            ChooseColourFor = chooseColourFor;
        }

        public ChooseColourFor? ChooseColourFor { get; set; }
    }
}