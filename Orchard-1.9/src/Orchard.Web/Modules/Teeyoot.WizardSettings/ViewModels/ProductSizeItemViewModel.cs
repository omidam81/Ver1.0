namespace Teeyoot.WizardSettings.ViewModels
{
    public class ProductSizeItemViewModel
    {
        public int Id { get; set; }
        public double LengthCm { get; set; }
        public double WidthCm { get; set; }
        public double? SleeveCm { get; set; }
        public double LengthInch { get; set; }
        public double WidthInch { get; set; }
        public double? SleeveInch { get; set; }
        public int SizeCodeId { get; set; }
        public string SizeCodeName { get; set; }
        public bool Selected { get; set; }
    }
}