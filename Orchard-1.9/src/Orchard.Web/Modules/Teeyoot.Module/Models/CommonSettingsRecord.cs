namespace Teeyoot.Module.Models
{
    public class CommonSettingsRecord
    {
        public virtual int Id { get; protected set; }
        public virtual bool DoNotAcceptAnyNewCampaigns { get; set; }
        public virtual string CommonCulture { get; set; }
        //
        public virtual string CashOnDeliveryAvailabilityMessage { get; set; }
        public virtual string CheckoutPageRightSideContent { get; set; }

        public virtual CountryRecord CountryRecord { get; set; }
    }
}
