using System.ComponentModel.DataAnnotations;
namespace Teeyoot.Module.Models
{
    public class DeliverySettingRecord
    {
        public virtual int Id { get; protected set; }

        public virtual string State { get; set; }

        //todo: (auth:juiceek) drop this field after relinking business logic to new Cost fields
        public virtual double DeliveryCost { get; set; }

        public virtual bool Enabled { get; set; }

        public virtual string DeliveryCulture { get; set; }


        public virtual CountryRecord Country { get; set; }

        public virtual double PostageCost { get; set; }

        public virtual double CodCost { get; set; }


    }
}