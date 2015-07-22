using Orchard.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CampaignCategoriesPart : ContentPart<CampaignCategoriesRecord>
    {
        public string Name
        {
            get
            {
                return Retrieve(p => p.Name);
            }
            set
            {
                Store(p => p.Name, value);
            }
        }

        public bool IsVisible
        {
            get
            {
                return Retrieve(p => p.IsVisible);
            }
            set
            {
                Store(p => p.IsVisible, value);
            }
        }
    }
}