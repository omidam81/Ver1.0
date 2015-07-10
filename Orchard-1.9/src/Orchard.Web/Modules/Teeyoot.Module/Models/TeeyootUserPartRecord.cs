using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class TeeyootUserPartRecord : ContentPartRecord
    {
        public virtual DateTime CreatedUtc { get; set; }

        public virtual string PublicName { get; set; }

        public virtual string PhoneNumber { get; set; }
    }
}