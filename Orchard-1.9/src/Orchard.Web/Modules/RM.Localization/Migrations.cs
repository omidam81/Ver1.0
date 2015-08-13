using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace RM.Localization
{
    [OrchardFeature("RM.Localization.CookieCultureSelector")]
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(
                "CookieCulturePickerPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "CookieCulturePicker",
                cfg => cfg
                           .WithPart("CookieCulturePickerPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );
            return 1;
        }
    }

    [OrchardFeature("RM.Localization.ShadowCultureManager")]
    public class ShadowCultureManagerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ShadowCulturePartRecord",
                table => table.ContentPartRecord()
                    .Column<string>("Rules", c=>c.WithLength(2048))
                );
            return 1;
        }
    }
}
