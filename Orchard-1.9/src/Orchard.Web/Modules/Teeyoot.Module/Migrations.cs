﻿using Orchard.Data.Migration;
using Orchard.Core.Contents.Extensions;
using Orchard.ContentManagement.MetaData;
using System;
using Teeyoot.Module.Models;
using System.Data;

namespace Teeyoot.Module
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ProductImageRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Width")
                    .Column<int>("Height")
                    .Column<int>("Ppi")
                    .Column<int>("PrintableFrontLeft")
                    .Column<int>("PrintableFrontTop")
                    .Column<int>("PrintableFrontWidth")
                    .Column<int>("PrintableFrontHeight")
                    .Column<int>("ChestLineFront")
                    .Column<int>("PrintableBackLeft")
                    .Column<int>("PrintableBackTop")
                    .Column<int>("PrintableBackWidth")
                    .Column<int>("PrintableBackHeight")
                    .Column<int>("ChestLineBack")
                    .Column<string>("Gender", c => c.Nullable())

            );

            SchemaBuilder.CreateTable(typeof(ProductGroupRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.CreateTable(typeof(ProductHeadlineRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.CreateTable(typeof(ProductRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(250))
                    .Column<int>("ProductGroupRecord_Id")
                    .Column<int>("ProductHeadlineRecord_Id")
                    .Column<int>("ProductImageRecord_Id")
                    .Column<string>("Materials", c => c.Nullable())
                    .Column<string>("Details", c => c.Unlimited().Nullable())
            );

            SchemaBuilder.CreateTable(typeof(ProductColorRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
                    .Column<string>("Value", c => c.WithLength(10))
                    .Column<int>("Importance", c => c.Nullable())
            );

            SchemaBuilder.CreateTable(typeof(SizeCodeRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(10))
            );

            SchemaBuilder.CreateTable(typeof(ProductSizeRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("SizeCodeRecord_Id")
                    .Column<double>("LengthCm")
                    .Column<double>("WidthCm")
                    .Column<double>("SleeveCm", c => c.Nullable())
                    .Column<double>("LengthInch")
                    .Column<double>("WidthInch")
                    .Column<double>("SleeveInch", c => c.Nullable())
            );

            SchemaBuilder.CreateTable(typeof(LinkProductColorRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProductColorRecord_Id")
                    .Column<int>("ProductRecord_Id")
                    .Column<double>("BaseCost")
            );

            SchemaBuilder.CreateTable(typeof(LinkProductSizeRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProductSizeRecord_Id")
                    .Column<int>("ProductRecord_Id")
            );

            SchemaBuilder.CreateTable(typeof(TeeyootUserPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("PublicName", c => c.WithLength(50))
                    .Column<string>("PhoneNumber", c => c.WithLength(50))
                    .Column<DateTime>("CreatedUtc")
            );

            SchemaBuilder.CreateTable(typeof(CurrencyRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Code", c => c.WithLength(10))
                    .Column<string>("Name", c => c.WithLength(150))
                    .Column<string>("ShortName", c => c.WithLength(50))
            );

            SchemaBuilder.CreateTable(typeof(CampaignRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Title")
                    .Column<int>("TeeyootUserId")
                    .Column<int>("ProductCountGoal")
                    .Column<int>("ProductCountSold")
                    .Column<string>("Design", c => c.Unlimited())
                    .Column<string>("Description", c => c.Unlimited())
                    .Column<DateTime>("EndDate")
                    .Column<string>("URL")
                    .Column<bool>("AllowPickUpOrdersFromMe", c => c.WithDefault(false))
                    .Column<bool>("BackSideByDefault", c => c.WithDefault(false))
            );

            SchemaBuilder.CreateTable(typeof(CampaignProductRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CampaignRecord_Id")
                    .Column<int>("CurrencyRecord_Id")
                    .Column<double>("Price")
                    .Column<double>("BaseCost")
                    .Column<int>("ProductRecord_Id")
                    .Column<int>("ProductColorRecord_Id")
            );

            SchemaBuilder.CreateTable(typeof(FontRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Family", c => c.WithLength(150))
                    .Column<string>("FileName", c => c.WithLength(150))
                    .Column<string>("Tags", c => c.WithLength(250))
                    .Column<int>("Priority", c => c.Nullable())
            );

            SchemaBuilder.CreateTable(typeof(SwatchRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
                    .Column<bool>("InStock", c => c.WithDefault(true))
                    .Column<int>("Red")
                    .Column<int>("Blue")
                    .Column<int>("Green")
            );

            SchemaBuilder.CreateForeignKey("Product_ProductGroup", "ProductRecord", new[] { "ProductGroupRecord_Id" }, "ProductGroupRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("Product_ProductHeadline", "ProductRecord", new[] { "ProductHeadlineRecord_Id" }, "ProductHeadlineRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("Product_ProductImage", "ProductRecord", new[] { "ProductImageRecord_Id" }, "ProductImageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ProductSize_SizeCode", "ProductSizeRecord", new[] { "SizeCodeRecord_Id" }, "SizeCodeRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductColor_Product", "LinkProductColorRecord", new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductColor_ProductColor", "LinkProductColorRecord", new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductSize_Product", "LinkProductSizeRecord", new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductSize_ProductSize", "LinkProductSizeRecord", new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Campaign", "CampaignProductRecord", new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Product", "CampaignProductRecord", new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColor", "CampaignProductRecord", new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("Campaign_TeeyootUser", "CampaignRecord", new[] { "TeeyootUserId" }, "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Currency", "CampaignProductRecord", new[] { "CurrencyRecord_Id" }, "CurrencyRecord", new[] { "Id" });


            ContentDefinitionManager.AlterPartDefinition(typeof(TeeyootUserPart).Name, part => part
                .Attachable(false)
                );

            ContentDefinitionManager.AlterTypeDefinition("TeeyootUser", type => type
                .WithPart(typeof(TeeyootUserPart).Name)
                .WithPart("UserPart")
                );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table
                    .AddColumn<string>("Alias", c => c.WithLength(100))
            );

            SchemaBuilder.CreateTable(typeof(OrderRecord).Name,
                 table => table
                     .Column<int>("Id", column => column.PrimaryKey().Identity())
                     .Column<string>("Email", c => c.WithLength(100))
                     .Column<string>("FirstName", c => c.WithLength(100))
                     .Column<string>("LastName", c => c.WithLength(100))
                     .Column<string>("City", c => c.WithLength(100))
                     .Column<string>("State", c => c.WithLength(100))
                     .Column<string>("Country", c => c.WithLength(100))
                     .Column<double>("TotalPrice")
                     .Column<int>("CurrencyRecord_Id")
                     );

            SchemaBuilder.CreateTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("ApiKey", c => c.WithLength(50))
                    .Column<string>("MailChimpCampaignId", c => c.WithLength(50))
                    .Column<string>("TemplateId", c => c.WithLength(50))
                    .Column<string>("MailChimpListId", c => c.WithLength(50))
                    .Column<string>("Culture", c => c.WithLength(50))
            );

            SchemaBuilder.CreateTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("OrderRecord_Id")
                    .Column<int>("CampaignProductRecord_Id")
                    .Column<int>("Count")
                    .Column<string>("Size", c => c.WithLength(10)));

            SchemaBuilder.CreateForeignKey("Order_Currency", "OrderRecord", new[] { "CurrencyRecord_Id" }, "CurrencyRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_Order", "LinkOrderCampaignProductRecord", new[] { "OrderRecord_Id" }, "OrderRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_CampaignProduct", "LinkOrderCampaignProductRecord", new[] { "CampaignProductRecord_Id" }, "CampaignProductRecord", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(typeof(MailChimpSettingsPart).Name, part => part
               .Attachable(false)
               );

            ContentDefinitionManager.AlterTypeDefinition("MailChimpSettings", type => type
                .WithPart(typeof(MailChimpSettingsPart).Name)
                .WithPart("CommonPart")
                );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
               table => table
                   .AddColumn<string>("TemplateName", c => c.WithLength(100))
           );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
            .AlterColumn("TemplateId", x => x.WithType(DbType.Int32))
            );

            return 5;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table
                    .AddColumn<string>("Alias", c => c.WithLength(100))
            );

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.CreateTable(typeof(OrderRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Email", c => c.WithLength(100))
                    .Column<string>("FirstName", c => c.WithLength(100))
                    .Column<string>("LastName", c => c.WithLength(100))
                    .Column<string>("City", c => c.WithLength(100))
                    .Column<string>("State", c => c.WithLength(100))
                    .Column<string>("Country", c => c.WithLength(100))
                    .Column<double>("TotalPrice")
                    .Column<int>("CurrencyRecord_Id")
                    );

            SchemaBuilder.CreateTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("ApiKey", c => c.WithLength(50))
                    .Column<string>("MailChimpCampaignId", c => c.WithLength(50))
                    .Column<string>("TemplateId", c => c.WithLength(50))
                    .Column<string>("MailChimpListId", c => c.WithLength(50))
                    .Column<string>("Culture", c => c.WithLength(50))
            );

            SchemaBuilder.CreateTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("OrderRecord_Id")
                    .Column<int>("CampaignProductRecord_Id")
                    .Column<int>("Count")
                    .Column<string>("Size", c => c.WithLength(10)));

            SchemaBuilder.CreateForeignKey("Order_Currency", "OrderRecord", new[] { "CurrencyRecord_Id" }, "CurrencyRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_Order", "LinkOrderCampaignProductRecord", new[] { "OrderRecord_Id" }, "OrderRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_CampaignProduct", "LinkOrderCampaignProductRecord", new[] { "CampaignProductRecord_Id" }, "CampaignProductRecord", new[] { "Id" });

            ContentDefinitionManager.AlterPartDefinition(typeof(MailChimpSettingsPart).Name, part => part
               .Attachable(false)
               );

            ContentDefinitionManager.AlterTypeDefinition("MailChimpSettings", type => type
                .WithPart(typeof(MailChimpSettingsPart).Name)
                .WithPart("CommonPart")
                );

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("TemplateName", c => c.WithLength(100))                                      
            );

            SchemaBuilder.AlterTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
            .AlterColumn("TemplateId", x => x.WithType(DbType.Int32))
            );

            return 5;
        }
    }
}