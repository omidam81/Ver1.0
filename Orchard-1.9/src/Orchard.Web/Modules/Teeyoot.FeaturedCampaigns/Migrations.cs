using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Teeyoot.FeaturedCampaigns
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(
                "CampaignCategoriesWidgetPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "CampaignCategoriesWidget",
                cfg => cfg
                           .WithPart("CampaignCategoriesWidgetPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }
    }
}
