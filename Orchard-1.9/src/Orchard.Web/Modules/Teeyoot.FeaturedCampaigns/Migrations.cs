using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(
                "FeaturedCampaignsWidgetPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "FeaturedCampaignsWidget",
                cfg => cfg
                           .WithPart("FeaturedCampaignsWidgetPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );

            return 2;
        }

        public int UpdateFrom1(){
            ContentDefinitionManager.AlterPartDefinition(
                "FeaturedCampaignsWidgetPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "FeaturedCampaignsWidget",
                cfg => cfg
                           .WithPart("FeaturedCampaignsWidgetPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );

            return 2;
        }
    }
}
