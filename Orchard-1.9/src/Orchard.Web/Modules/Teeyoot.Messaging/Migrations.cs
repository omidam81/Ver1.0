using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System.Data;
using Teeyoot.Messaging.Models;

namespace Teeyoot.Messaging
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("ApiKey", c => c.WithLength(50))
                    .Column<string>("MailChimpCampaignId", c => c.WithLength(50))
                    .Column<int>("TemplateId")
                    .Column<string>("MailChimpListId", c => c.WithLength(50))
                    .Column<string>("Culture", c => c.WithLength(50))
                    .Column<string>("TemplateName", c => c.WithLength(100))
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(MailChimpSettingsPart).Name, part => part
               .Attachable(false)
               );

            ContentDefinitionManager.AlterTypeDefinition("MailChimpSettings", type => type
                .WithPart(typeof(MailChimpSettingsPart).Name)
                .WithPart("CommonPart")
                );

            return 1;
        }
    }
}