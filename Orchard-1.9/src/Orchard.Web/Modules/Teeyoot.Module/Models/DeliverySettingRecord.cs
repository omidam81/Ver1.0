using System.ComponentModel.DataAnnotations;
namespace Teeyoot.Module.Models
{
    public class DeliverySettingRecord
    {
        public virtual int Id { get; protected set; }

        public virtual string State { get; set; }

        public virtual double DeliveryCost { get; set; }

        public virtual bool Enabled { get; set; }
    }
}