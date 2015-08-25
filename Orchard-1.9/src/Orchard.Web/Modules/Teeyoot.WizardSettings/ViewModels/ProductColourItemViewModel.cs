namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductColourItemViewModel
    {
        public int Id { get; set; }
        public int ProductColourId { get; set; }
        public string Name { get; set; }
        public string HexValue { get; set; }
        //public float BaseCost { get; set; }
        public bool Selected { get; set; }
    }
}