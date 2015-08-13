using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using RM.Localization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RM.Localization.Drivers
{
    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCulturePartDriver : ContentPartDriver<ShadowCulturePart>
    {
        protected override string Prefix { get { return "ShadowCultureSettings"; } }

        protected override DriverResult Editor(ShadowCulturePart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_ShadowCulture_Settings",
                               () => shapeHelper.EditorTemplate(TemplateName: "Parts.ShadowCulture.Settings", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(ShadowCulturePart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}
