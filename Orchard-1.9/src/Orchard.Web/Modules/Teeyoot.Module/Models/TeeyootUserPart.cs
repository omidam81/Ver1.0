using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class TeeyootUserPart : ContentPart<TeeyootUserPartRecord>
    {
        public DateTime CreatedUtc
        {
            get
            {
                return Retrieve(p => p.CreatedUtc);
            }
            set
            {
                Store(p => p.CreatedUtc, value);
            }
        }
        public string PublicName
        {
            get
            {
                return Retrieve(p => p.PublicName);
            }
            set
            {
                Store(p => p.PublicName, value);
            }
        }
        public string PhoneNumber
        {
            get
            {
                return Retrieve(p => p.PhoneNumber);
            }
            set
            {
                Store(p => p.PhoneNumber, value);
            }
        }
    }
}