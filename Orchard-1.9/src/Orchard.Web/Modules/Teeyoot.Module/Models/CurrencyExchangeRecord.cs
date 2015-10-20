namespace Teeyoot.Module.Models
{
    public class CurrencyExchangeRecord
    {
        public virtual int Id { get; protected set; }
        public virtual CurrencyRecord CurrencyFrom { get; set; }
        public virtual CurrencyRecord CurrencyTo { get; set; }
        public virtual double RateForBuyer { get; set; }
        public virtual double RateForSeller { get; set; }
    }
}