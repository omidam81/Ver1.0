using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using RM.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.Localization.Handlers
{
    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCulturePartHandler : ContentHandler
    {
        public Localizer T { get; set; }

        public ShadowCulturePartHandler(IRepository<ShadowCulturePartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<ShadowCulturePart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            T = NullLocalizer.Instance;
        }
    }
}
