using System.ComponentModel.DataAnnotations;
namespace Teeyoot.Module.ViewModels
{
    public class EditDeliverySettingViewModel
    {
        public int Id { get; set; }

        public string State { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public  double DeliveryCost { get; set; }



    }
}