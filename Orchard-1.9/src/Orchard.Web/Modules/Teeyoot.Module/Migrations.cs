using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using Teeyoot.Module.Models;

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
                    .Column<bool>("IsForCharity", c => c.WithDefault(false))
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

            SchemaBuilder.CreateTable(typeof(CampaignCategoriesRecord).Name,
                table => table
                .ContentPartRecord()
                .Column<string>("Name", c => c.WithLength(50))
            );

            ContentDefinitionManager.AlterPartDefinition(typeof(CampaignCategoriesPart).Name, part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("CampaignCategories", type => type
                .WithPart(typeof(CampaignCategoriesPart).Name)
                .WithPart("CommonPart")
            );

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

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("Tags", c => c.Unlimited()));

            SchemaBuilder.CreateTable(typeof(LinkProductGroupRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProductGroupRecord_Id")
                    .Column<int>("ProductRecord_Id")
            );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("StreetAddress"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PostalCode"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PhoneNumber"));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Created", c => c.NotNull()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Paid", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(CampaignStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CampaignStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Campaign_Status", "CampaignRecord", new[] { "CampaignStatusRecord_Id" }, "CampaignStatusRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("Size"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("SizeId"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "SizeId" }, "ProductSizeRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<DateTime>("StartDate"));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("Tags"));

            SchemaBuilder.AlterTable(typeof(CampaignCategoriesRecord).Name, table => table.AddColumn<bool>("IsVisible"));

            SchemaBuilder.CreateTable(typeof(LinkCampaignAndCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("CampaignRecord_Id")
                .Column<int>("CampaignCategoriesPartRecord_Id")
            );

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_Campaign", "LinkCampaignAndCategoriesRecord", new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories", "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesPartRecord", new[] { "Id" });

            SchemaBuilder.DropForeignKey("LinkOrderCampaignProductRecord", "OrderProduct_Size");

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                 table => table.DropColumn("SizeId"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("ProductSizeRecord_Id"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(StoreRecord).Name,
                 table => table
                     .Column<int>("Id", column => column.PrimaryKey().Identity())
                     .Column<int>("TeeyootUserId")
                     .Column<string>("Title")
                     .Column<string>("Description")
                     .Column<bool>("HideStore", c => c.WithDefault(false))
                     .Column<bool>("CrossSelling", c => c.WithDefault(false))
                     );

            SchemaBuilder.CreateTable(typeof(LinkStoreCampaignRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("StoreRecord_Id")
                    .Column<int>("CampaignRecord_Id")
                    );

            SchemaBuilder.CreateForeignKey("Store_TeeyootUser", "StoreRecord", new[] { "TeeyootUserId" }, "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkStoreCampaignRecord_CampaignRecord", "LinkStoreCampaignRecord", new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_StoreRecord", "LinkStoreCampaignRecord", new[] { "StoreRecord_Id" }, "StoreRecord", new[] { "Id" });

            SchemaBuilder.DropForeignKey("LinkCampaignAndCategoriesRecord", "LinkCampaignAndCategories_CampaignCategories");
            SchemaBuilder.DropTable(typeof(CampaignCategoriesRecord).Name);
            ContentDefinitionManager.DeletePartDefinition(typeof(CampaignCategoriesPart).Name);
            ContentDefinitionManager.DeleteTypeDefinition("CampaignCategories");

            SchemaBuilder.CreateTable(typeof(CampaignCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Name", column => column.WithLength(50))
                .Column<bool>("IsVisible")
            );

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories", "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
               table => table.AddColumn<bool>("IsFeatured", cl => cl.WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(StoreRecord).Name,
                 table => table.AddColumn<string>("Url", cl => cl.WithLength(150)));

            SchemaBuilder.CreateTable(typeof(TShirtCostRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<float>("FirstScreenCost")
                    .Column<float>("AdditionalScreenCosts")
                    .Column<float>("InkCost")
                    .Column<int>("PrintsPerLitre")
                    .Column<float>("LabourCost")
                    .Column<int>("LabourTimePerColourPerPrint")
                    .Column<int>("LabourTimePerSidePrintedPerPrint")
                    .Column<float>("CostOfMaterial")
                    .Column<float>("PercentageMarkUpRequired")
                    .Column<float>("DTGPrintPrice")
            );

            SchemaBuilder.CreateTable(typeof(OrderStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Reserved", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<int>("OrderStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Order_Status", "OrderRecord", new[] { "OrderStatusRecord_Id" }, "OrderStatusRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(PayoutRecord).Name,
                     table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("Date")
                    .Column<string>("Event", c => c.Unlimited())
                    .Column<double>("Amount")
                    .Column<bool>("IsPlus")
                    .Column<int>("UserId")
            );

            SchemaBuilder.CreateForeignKey("PayoutRecord_UserId", "PayoutRecord", new[] { "UserId" }, "TeeyootUserPartRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Street", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Suit", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("City", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("State", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Zip", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("OrderPublicId", c => c.NotNull().WithLength(50)));

            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.DropColumn("CostOfMaterial"));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name, table => table.AddColumn<string>("Status", c => c.NotNull().WithLength(50)));

            SchemaBuilder.CreateTable(typeof(PromotionRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("Expiration")
                    .Column<string>("PromoId", c => c.WithLength(15))
                    .Column<string>("AmountType", c => c.WithLength(50))
                    .Column<int>("AmountSize")
                    .Column<string>("DiscountType", c => c.WithLength(50))
                    .Column<bool>("Status")
                    .Column<int>("Redeemed")
            ); 

            return 29;

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

            return 4;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("Tags", c => c.Unlimited()));

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.DropForeignKey("ProductRecord", "Product_ProductGroup");

            SchemaBuilder.AlterTable(typeof(ProductRecord).Name, table => table.DropColumn("ProductGroupRecord_Id"));

            SchemaBuilder.CreateTable(typeof(LinkProductGroupRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProductGroupRecord_Id")
                    .Column<int>("ProductRecord_Id")
            );

            SchemaBuilder.CreateForeignKey("LinkProductGroup_Product", "LinkProductGroupRecord", new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductGroup_ProductGroup", "LinkProductGroupRecord", new[] { "ProductGroupRecord_Id" }, "ProductGroupRecord", new[] { "Id" });

            return 7;
        }
        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("StreetAddress"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PostalCode"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PhoneNumber"));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));


            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Created", c => c.NotNull()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Paid", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(CampaignStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CampaignStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Campaign_Status", "CampaignRecord", new[] { "CampaignStatusRecord_Id" }, "CampaignStatusRecord", new[] { "Id" });

            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("Size"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("SizeId"));

            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "SizeId" }, "ProductSizeRecord", new[] { "Id" });

            return 12;
        }

        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<DateTime>("StartDate"));

            return 13;
        }

        public int UpdateFrom13()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("Tags"));

            SchemaBuilder.AlterTable(typeof(CampaignCategoriesRecord).Name, table => table.AddColumn<bool>("IsVisible"));

            SchemaBuilder.CreateTable(typeof(LinkCampaignAndCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("CampaignRecord_Id")
                .Column<int>("CampaignCategoriesPartRecord_Id")
            );

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_Campaign", "LinkCampaignAndCategoriesRecord", new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories", "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesPartRecord", new[] { "Id" });

            return 14;
        }

        public int UpdateFrom14()
        {
            SchemaBuilder.DropForeignKey("LinkOrderCampaignProductRecord", "OrderProduct_Size");

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("SizeId"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("ProductSizeRecord_Id"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });

            return 15;
        }

        public int UpdateFrom15()
        {
            SchemaBuilder.CreateTable(typeof(StoreRecord).Name,
                 table => table
                     .Column<int>("Id", column => column.PrimaryKey().Identity())
                     .Column<int>("TeeyootUserId")
                     .Column<string>("Title")
                     .Column<string>("Description")
                     .Column<bool>("HideStore", c => c.WithDefault(false))
                     .Column<bool>("CrossSelling", c => c.WithDefault(false))
                     );

            SchemaBuilder.CreateTable(typeof(LinkStoreCampaignRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("StoreRecord_Id")
                    .Column<int>("CampaignRecord_Id")
                    );

            SchemaBuilder.CreateForeignKey("Store_TeeyootUser", "StoreRecord", new[] { "TeeyootUserId" }, "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkStoreCampaignRecord_CampaignRecord", "LinkStoreCampaignRecord", new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_StoreRecord", "LinkStoreCampaignRecord", new[] { "StoreRecord_Id" }, "StoreRecord", new[] { "Id" });

            return 16;
        }

        public int UpdateFrom16()
        {
            SchemaBuilder.DropForeignKey("LinkCampaignAndCategoriesRecord", "LinkCampaignAndCategories_CampaignCategories");

            SchemaBuilder.DropTable(typeof(CampaignCategoriesRecord).Name);
            ContentDefinitionManager.DeletePartDefinition(typeof(CampaignCategoriesPart).Name);
            ContentDefinitionManager.DeleteTypeDefinition("CampaignCategories");

            SchemaBuilder.CreateTable(typeof(CampaignCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Name", column => column.WithLength(50))
                .Column<bool>("IsVisible")
            );

            return 17;
        }

        public int UpdateFrom17()
        {
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories", "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesRecord", new[] { "Id" });

            return 18;
        }

        public int UpdateFrom18()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsFeatured", cl => cl.WithDefault(false)));

            return 19;
        }
        public int UpdateFrom19()
        {
            SchemaBuilder.AlterTable(typeof(StoreRecord).Name,
                table => table.AddColumn<string>("Url", cl => cl.WithLength(150)));

            return 20;
        }

        public int UpdateFrom20()
        {
            SchemaBuilder.CreateTable(typeof(TShirtCostRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<float>("FirstScreenCost")
                    .Column<float>("AdditionalScreenCosts")
                    .Column<float>("InkCost")
                    .Column<int>("PrintsPerLitre")
                    .Column<float>("LabourCost")
                    .Column<int>("LabourTimePerColourPerPrint")
                    .Column<int>("LabourTimePerSidePrintedPerPrint")
                    .Column<float>("CostOfMaterial")
                    .Column<float>("PercentageMarkUpRequired")
                    .Column<float>("DTGPrintPrice")
            );

            return 21;
        }

        public int UpdateFrom21()
        {
            SchemaBuilder.CreateTable(typeof(OrderStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
            );

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<DateTime>("Reserved", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<int>("OrderStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Order_Status", "OrderRecord", new[] { "OrderStatusRecord_Id" }, "OrderStatusRecord", new[] { "Id" });

            return 22;
        }

        public int UpdateFrom22()
        {
            SchemaBuilder.CreateTable(typeof(PayoutRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("Date")
                    .Column<string>("Event", c => c.Unlimited())
                    .Column<double>("Amount")
                    .Column<bool>("IsPlus")
                    .Column<int>("UserId")
            );

            SchemaBuilder.CreateForeignKey("PayoutRecord_UserId", "PayoutRecord", new[] { "UserId" }, "TeeyootUserPartRecord", new[] { "Id" });

            return 23;
        }

        public int UpdateFrom23()
        {
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Street", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Suit", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("City", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("State", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name, table => table.AddColumn<string>("Zip", c => c.WithLength(50)));

            return 24;
        }

        public int UpdateFrom24()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<string>("OrderPublicId", c => c.NotNull().WithLength(50)));
            return 25;
        }

        public int UpdateFrom25()
        {
            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.DropColumn("CostOfMaterial"));
            return 26;
        }
        
        public int UpdateFrom26()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(false)));
            return 27;
        }

        public int UpdateFrom27()
        {
            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name, table => table.AddColumn<string>("Status", c => c.NotNull().WithLength(50)));
            return 28;
        }

        public int UpdateFrom28()
        {
            SchemaBuilder.CreateTable(typeof(PromotionRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("Expiration")
                    .Column<string>("PromoId", c => c.WithLength(15))
                    .Column<string>("AmountType", c => c.WithLength(50))
                    .Column<int>("AmountSize")
                    .Column<string>("DiscountType", c => c.WithLength(50))
                    .Column<bool>("Status")
                    .Column<int>("Redeemed")
            ); 
            return 29;
        }
    }
}