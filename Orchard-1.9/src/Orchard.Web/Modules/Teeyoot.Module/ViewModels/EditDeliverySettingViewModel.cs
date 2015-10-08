using System.ComponentModel.DataAnnotations;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.ViewModels
{
    public class EditDeliverySettingViewModel
    {
        public int Id { get; set; }

        public string State { get; set; }

        //todo: (auth:juiceek) drop this field
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public  double DeliveryCost { get; set; }

        public int CountryId { get; set; }

        public double PostageCost { get; set; }

        public double CodCost { get; set; }

    }
}