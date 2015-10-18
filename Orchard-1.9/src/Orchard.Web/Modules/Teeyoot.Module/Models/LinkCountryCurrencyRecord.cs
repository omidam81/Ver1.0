namespace Teeyoot.Module.Models
{
    public class LinkCountryCurrencyRecord
    {
        public virtual int Id { get; protected set; }
        public virtual CurrencyRecord CurrencyRecord { get; set; }
        public virtual CountryRecord CountryRecord { get; set; }
    }
}