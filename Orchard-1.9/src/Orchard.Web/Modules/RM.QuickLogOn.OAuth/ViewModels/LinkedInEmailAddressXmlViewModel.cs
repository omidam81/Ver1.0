using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace RM.QuickLogOn.OAuth.ViewModels
{
    [XmlType("person")]
    public class LinkedInEmailAddressXmlViewModel
    {
        [XmlElement]
        public string id { get; set; }
        [XmlElement("first-name")]
        public string first_name { get; set; }
        [XmlElement("last-name")]
        public string last_name { get; set; }
        [XmlElement("email-address")]
        public string email_address { get; set; }
    }
}
