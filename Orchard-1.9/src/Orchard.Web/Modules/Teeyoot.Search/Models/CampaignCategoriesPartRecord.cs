﻿using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Search.Models
{
    public class CampaignCategoriesPartRecord : ContentPartRecord
    {
        public virtual string Name { get; set; }
    }
}