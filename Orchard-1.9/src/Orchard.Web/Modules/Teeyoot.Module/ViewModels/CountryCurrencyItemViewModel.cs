namespace Teeyoot.Module.ViewModels
{
    public class CountryCurrencyItemViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int? CountryId { get; set; }
        public string CountryName { get; set; }
        public string FlagFileName { get; set; }
    }
}