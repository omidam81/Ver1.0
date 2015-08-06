using System.Runtime.Serialization;

namespace Teeyoot.Account.Common
{
    [DataContract]
    public class FacebookTokenInspectionJsonViewModel
    {
        [DataMember]
        // ReSharper disable once InconsistentNaming
        public TokenData data { get; set; }
    }

    [DataContract]
    public class TokenData
    {
        [DataMember]
        // ReSharper disable once InconsistentNaming
        public string app_id { get; set; }
    }
}