using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CampaignCategoriesRecord
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }

        public virtual bool IsVisible { get; set; }

        public virtual string CategoriesCulture { get; set; }
        public virtual CountryRecord CountryRecord { get; set; }
    }
}