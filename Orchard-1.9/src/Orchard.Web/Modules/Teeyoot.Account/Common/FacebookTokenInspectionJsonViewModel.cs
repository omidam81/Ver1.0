using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace Teeyoot.Account.Common
{
    [DataContract]
    public class FacebookTokenInspectionJsonViewModel
    {
        [DataMember]
        public TokenData data { get; set; }
    }

    [DataContract]
    public class TokenData
    {
        [DataMember]
        public string app_id { get; set; }
    }
}