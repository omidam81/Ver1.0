using Orchard.Data.Migration;
using Orchard.Core.Contents.Extensions;
using Orchard.ContentManagement.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.FAQ.Models;

namespace Teeyoot.FAQ
{
    public class Mirgations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(LanguageRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Code", c => c.WithLength(10))
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.CreateTable(typeof(FaqSectionRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.CreateTable(typeof(FaqEntryPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("Question", c => c.Unlimited())
                    .Column<string>("Answer", c => c.Unlimited())
                    .Column<int>("LanguageRecord_Id")
                    .Column<int>("FaqSectionRecord_Id")
            );

            SchemaBuilder.CreateForeignKey("FaqEntry_Language", typeof(FaqEntryPartRecord).Name, new[] { "LanguageRecord_Id" }, typeof(LanguageRecord).Name, new[] { "Id" });
            SchemaBuilder.CreateForeignKey("FaqEntry_FaqSection", typeof(FaqEntryPartRecord).Name, new[] { "FaqSectionRecord_Id" }, typeof(FaqSectionRecord).Name, new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(typeof(FaqEntryPart).Name, part => part
                .Attachable()
                );

            ContentDefinitionManager.AlterTypeDefinition("FaqEntry", type => type
                .WithPart(typeof(FaqEntryPart).Name)
                .WithPart("CommonPart")
                .WithPart("BodyPart")
                );

            return 2;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("FaqEntry", type => type
                .WithPart("BodyPart")
                );

            return 2;
        }
    }
}