using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Localization.Records;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Extensions;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCulturePartRecord : ContentPartRecord
    {
        public virtual string Rules { get; set; }
    }
}
