namespace Teeyoot.Localization.GeoLocation
{
    public class GeoLocationInfo
    {
        public string CountryIsoCode { get; set; }
        public LocationInfoStatus Status { get; private set; }
        public string StatusReason { get; private set; }

        internal GeoLocationInfo(LocationInfoStatus status, string statusReason)
        {
            Status = status;
            StatusReason = statusReason;
        }

        public GeoLocationInfo()
        {
            Status = LocationInfoStatus.LocationFound;
        }
    }
}
