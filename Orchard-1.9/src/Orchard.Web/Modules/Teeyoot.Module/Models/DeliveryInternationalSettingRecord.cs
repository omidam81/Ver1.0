namespace Teeyoot.Module.Models
{
    public class DeliveryInternationalSettingRecord
    {
        public virtual int Id { get; protected set; }
        public virtual CountryRecord CountryFrom { get; set; }
        public virtual CountryRecord CountryTo { get; set; }
        public virtual double DeliveryPrice { get; set; }
        public virtual bool IsActive { get; set; }
    }
}