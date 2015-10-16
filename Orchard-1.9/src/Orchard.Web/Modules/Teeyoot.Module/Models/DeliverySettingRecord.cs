namespace Teeyoot.Module.Models
{
    /// <summary>
    /// This class is used FOR DOMESTIC delivery settings only. 
    /// </summary>
    public class DeliverySettingRecord
    {
        public virtual int Id { get; protected set; }
        public virtual string State { get; set; }
        //todo: (auth:juiceek) Legacy property. Drop it after relinking business logic to new Cost fields.
        public virtual double DeliveryCost { get; set; }
        public virtual bool Enabled { get; set; }
        //todo: (aut:juiceek) Legacy property. Drop this after applying new logic.
        public virtual string DeliveryCulture { get; set; }
        public virtual CountryRecord Country { get; set; }
        public virtual double PostageCost { get; set; }
        public virtual double CodCost { get; set; }
    }
}