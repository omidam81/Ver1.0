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

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
               table => table
                   .DropColumn("MailChimpCampaignId")
           );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .DropColumn("TemplateId")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .DropColumn("TemplateName")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("WelcomeCampaignId", c => c.WithLength(50))
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<int>("WelcomeTemplateId")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("AllBuyersCampaignId", c => c.WithLength(50))
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<int>("AllBuyersTemplateId")
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

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .DropColumn("MailChimpCampaignId")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .DropColumn("TemplateId")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .DropColumn("TemplateName")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("WelcomeCampaignId", c => c.WithLength(50))
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<int>("WelcomeTemplateId")
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("AllBuyersCampaignId", c => c.WithLength(50))
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<int>("AllBuyersTemplateId")
            );

            return 2;
        }
    }
}