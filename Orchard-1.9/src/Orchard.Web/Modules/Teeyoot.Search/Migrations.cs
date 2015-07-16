using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Teeyoot.Search.Models;

namespace Teeyoot.Search
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CampaignCategoriesPartRecord).Name,
                table => table
                .ContentPartRecord()
                .Column<string>("Name", c => c.WithLength(50))
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(CampaignCategoriesPart).Name, part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("CampaignCategories", type => type
                .WithPart(typeof(CampaignCategoriesPart).Name)
                .WithPart("CommonPart")
            );

            return 1;
        }
    }
}