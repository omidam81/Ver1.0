namespace Teeyoot.Localization.GeoLocation
{
    internal class GeoLocationInfo
    {
        public string CountryIsoCode { get; set; }
        public LocationInfoStatus Status { get; private set; }
        public string StatusReason { get; private set; }

        public GeoLocationInfo(LocationInfoStatus status, string statusReason)
        {
            Status = status;
            StatusReason = statusReason;
        }

        public GeoLocationInfo(string countryIsoCode)
        {
            CountryIsoCode = countryIsoCode;
            Status = LocationInfoStatus.LocationFound;
        }
    }
}
