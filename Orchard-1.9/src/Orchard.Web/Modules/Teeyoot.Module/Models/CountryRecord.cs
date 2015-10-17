using System.Collections.Generic;

namespace Teeyoot.Module.Models
{
    public class CountryRecord
    {
        public virtual int Id { get; protected set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<LinkCountryCurrencyRecord> Currencies { get; set; }
    }
}