using System;
using Orchard.ContentManagement.Records;

namespace Teeyoot.Module.Models
{
    public class TeeyootUserPartRecord : ContentPartRecord
    {
        public virtual DateTime CreatedUtc { get; set; }
        public virtual string PublicName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Street { get; set; }
        public virtual string Suit { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Country { get; set; }
        public virtual string TeeyootUserCulture { get; set; }
        public virtual CountryRecord CountryRecord { get; set; }
        public virtual CurrencyRecord CurrencyRecord { get; set; }
    }
}