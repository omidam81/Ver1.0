
namespace Teeyoot.Module.ViewModels
{

    public class DeliveryInternationalSettingViewModel
    {
        public int Id { get; set; }

        public int CountryFromId { get; set; }
        public string CountryFromName { get; set; }

        public int CountryToId { get; set; }
        public string CountryToName { get; set; }

        public double DeliveryPrice { get; set; }

        public bool IsActive { get; set; }
    }
}