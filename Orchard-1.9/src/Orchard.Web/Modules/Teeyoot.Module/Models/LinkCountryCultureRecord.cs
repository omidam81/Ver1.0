using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization.Records;

namespace Teeyoot.Module.Models
{
    public class LinkCountryCultureRecord
    {
        public virtual int Id { get; set; }

        public virtual CultureRecord CultureRecord { get; set; }

        public virtual CountryRecord CountryRecord { get; set; }
    }
}