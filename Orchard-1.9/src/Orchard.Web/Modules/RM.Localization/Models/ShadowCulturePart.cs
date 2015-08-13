using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.Localization.Models
{
    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCulturePart : ContentPart<ShadowCulturePartRecord>
    {
        public virtual string Rules { get { return Record.Rules; } set { Record.Rules = value; } }
    }
}
