using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;



namespace Teeyoot.Module.Models
{
    
    public class DeliveryInternationalSettingRecord
    {
        public virtual int Id { get; protected set; }

        public virtual CountryRecord CountryFrom { get; set; }

        public virtual CountryRecord CountryTo { get; set; }
    }
}