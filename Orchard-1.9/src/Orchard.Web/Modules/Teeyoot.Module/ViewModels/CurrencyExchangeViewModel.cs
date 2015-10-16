namespace Teeyoot.Module.ViewModels
{

    public class CurrencyExchangeViewModel
    {
        public int Id { get; set; }

        public int CurrencyFromId { get; set; }
        public string CurrencyFromCode { get; set; }

        public int CurrencyToId { get; set; }
        public string CurrencyToCode { get; set; }

        public string CurrencyFromFlagFileName { get; set; }
        public string CurrencyToFlagFileName { get; set; }

        public double RateForBuyer { get; set; }
        public double RateForSeller { get; set; }
    }
}