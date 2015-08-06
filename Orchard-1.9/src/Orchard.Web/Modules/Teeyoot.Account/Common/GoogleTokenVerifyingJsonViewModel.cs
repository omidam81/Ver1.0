using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace Teeyoot.Account.Common
{
    [DataContract]
    public class GoogleTokenVerifyingJsonViewModel
    {
        [DataMember]
        public string audience { get; set; }

        [DataMember]
        public string scope { get; set; }

        [DataMember]
        public string userid { get; set; }

        [DataMember]
        public string expires_in { get; set; }
    }
}