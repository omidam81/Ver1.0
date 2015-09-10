using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using Orchard.Data;
using Teeyoot.Module.Models;

namespace Teeyoot.Module
{
    public class Migrations : DataMigrationImpl
    {
        private readonly IRepository<CommonSettingsRecord> _commonSettingsRepository;

        public Migrations(IRepository<CommonSettingsRecord> commonSettingsRepository)
        {
            _commonSettingsRepository = commonSettingsRepository;
        }

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

            ContentDefinitionManager.AlterPartDefinition(typeof(CampaignCategoriesPart).Name,
                part => part.Attachable(false));
            ContentDefinitionManager.AlterTypeDefinition("CampaignCategories", type => type
                .WithPart(typeof(CampaignCategoriesPart).Name)
                .WithPart("CommonPart")
                );

            SchemaBuilder.CreateForeignKey("Product_ProductHeadline", "ProductRecord",
                new[] { "ProductHeadlineRecord_Id" }, "ProductHeadlineRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("Product_ProductImage", "ProductRecord", new[] { "ProductImageRecord_Id" },
                "ProductImageRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("ProductSize_SizeCode", "ProductSizeRecord", new[] { "SizeCodeRecord_Id" },
                "SizeCodeRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductColor_Product", "LinkProductColorRecord",
                new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductColor_ProductColor", "LinkProductColorRecord",
                new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductSize_Product", "LinkProductSizeRecord",
                new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductSize_ProductSize", "LinkProductSizeRecord",
                new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Campaign", "CampaignProductRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Product", "CampaignProductRecord",
                new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColor", "CampaignProductRecord",
                new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("Campaign_TeeyootUser", "CampaignRecord", new[] { "TeeyootUserId" },
                "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_Currency", "CampaignProductRecord",
                new[] { "CurrencyRecord_Id" }, "CurrencyRecord", new[] { "Id" });


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

            SchemaBuilder.CreateForeignKey("Order_Currency", "OrderRecord", new[] { "CurrencyRecord_Id" },
                "CurrencyRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_Order", "LinkOrderCampaignProductRecord",
                new[] { "OrderRecord_Id" }, "OrderRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_CampaignProduct", "LinkOrderCampaignProductRecord",
                new[] { "CampaignProductRecord_Id" }, "CampaignProductRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("Tags", c => c.Unlimited()));

            SchemaBuilder.CreateTable(typeof(LinkProductGroupRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ProductGroupRecord_Id")
                    .Column<int>("ProductRecord_Id")
                );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("StreetAddress"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PostalCode"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PhoneNumber"));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Created", c => c.NotNull()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Paid", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(CampaignStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
                );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<int>("CampaignStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Campaign_Status", "CampaignRecord", new[] { "CampaignStatusRecord_Id" },
                "CampaignStatusRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("Size"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("SizeId"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "SizeId" },
                "ProductSizeRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<DateTime>("StartDate"));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("Tags"));

            SchemaBuilder.AlterTable(typeof(CampaignCategoriesRecord).Name, table => table.AddColumn<bool>("IsVisible"));

            SchemaBuilder.CreateTable(typeof(LinkCampaignAndCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("CampaignRecord_Id")
                .Column<int>("CampaignCategoriesPartRecord_Id")
                );

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_Campaign", "LinkCampaignAndCategoriesRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories",
                "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" },
                "CampaignCategoriesPartRecord", new[] { "Id" });

            SchemaBuilder.DropForeignKey("LinkOrderCampaignProductRecord", "OrderProduct_Size");

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("SizeId"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("ProductSizeRecord_Id"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord",
                new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });

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

            SchemaBuilder.CreateForeignKey("Store_TeeyootUser", "StoreRecord", new[] { "TeeyootUserId" },
                "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkStoreCampaignRecord_CampaignRecord", "LinkStoreCampaignRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_StoreRecord", "LinkStoreCampaignRecord",
                new[] { "StoreRecord_Id" }, "StoreRecord", new[] { "Id" });

            SchemaBuilder.DropForeignKey("LinkCampaignAndCategoriesRecord",
                "LinkCampaignAndCategories_CampaignCategories");
            SchemaBuilder.DropTable(typeof(CampaignCategoriesRecord).Name);
            ContentDefinitionManager.DeletePartDefinition(typeof(CampaignCategoriesPart).Name);
            ContentDefinitionManager.DeleteTypeDefinition("CampaignCategories");

            SchemaBuilder.CreateTable(typeof(CampaignCategoriesRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("Name", column => column.WithLength(50))
                .Column<bool>("IsVisible")
                );

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories",
                "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesRecord",
                new[] { "Id" });

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

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Reserved", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<int>("OrderStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Order_Status", "OrderRecord", new[] { "OrderStatusRecord_Id" },
                "OrderStatusRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(PayoutRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("Date")
                    .Column<string>("Event", c => c.Unlimited())
                    .Column<double>("Amount")
                    .Column<bool>("IsPlus")
                    .Column<int>("UserId")
                );

            SchemaBuilder.CreateForeignKey("PayoutRecord_UserId", "PayoutRecord", new[] { "UserId" },
                "TeeyootUserPartRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Street", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Suit", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("City", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("State", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Zip", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("OrderPublicId", c => c.NotNull().WithLength(50)));

            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.DropColumn("CostOfMaterial"));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name,
                table => table.AddColumn<string>("Status", c => c.NotNull().WithLength(50)));

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

            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
                table => table.AddColumn<int>("Userid", c => c.NotNull()));

            SchemaBuilder.CreateForeignKey("PromotionRecord_UserId", "PromotionRecord", new[] { "UserId" },
                "TeeyootUserPartRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(OrderHistoryRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("EventDate")
                    .Column<string>("Event", c => c.Unlimited())
                    .Column<int>("OrderRecord_Id")
                );

            SchemaBuilder.CreateForeignKey("OrderHistory_Order", "OrderHistoryRecord", new[] { "OrderRecord_Id" },
                "OrderRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(PaymentInformationRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("AccountNumber")
                    .Column<string>("BankName")
                    .Column<int>("ContactNumber")
                    .Column<string>("MessAdmin")
                    .Column<int>("TranzactionId")
                );

            SchemaBuilder.CreateForeignKey("Tranzaction_Payout_Id", "PaymentInformationRecord", new[] { "TranzactionId" },
                "PayoutRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<double>("Promotion"));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<double>("TotalPriceWithPromo"));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
                table => table.AddColumn<string>("AccountHolderName"));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(true)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsApproved", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("TranzactionId", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(MessageRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("UserId")
                    .Column<string>("Text")
                    .Column<string>("From")
                    .Column<DateTime>("SendDate")
                );

            SchemaBuilder.CreateForeignKey("Message_User_Id", "MessageRecord", new[] { "UserId" }, "TeeyootUserPartRecord",
                new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("Delete", c => c.WithDefault(null)));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.DropColumn("Text"));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Text", c => c.Unlimited()));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("Delete"));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.DropColumn("From"));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Sender", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.AddColumn<int>("CampaignId"));

            SchemaBuilder.CreateForeignKey("Message_Campaign_Id", "MessageRecord", new[] { "CampaignId" },
                "CampaignRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsPrivate", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<bool>("IsApprowed", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Subject", c => c.WithLength(50)));

            SchemaBuilder.AlterTable(typeof(LinkProductSizeRecord).Name,
                table => table.AddColumn<float>("SizeCost", c => c.NotNull().WithDefault(float.Parse("0"))));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<bool>("ProfitPaid", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.CreateTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("ApiKey", c => c.WithLength(50))
                    .Column<string>("Culture", c => c.WithLength(50)));


            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("CampaignProfit", c => c.NotNull().WithDefault(string.Empty)));

            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name,
                table => table.AddColumn<int>("SalesGoal", c => c.NotNull().WithDefault(500)));

            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name,
                table => table.AddColumn<int>("MaxColors", c => c.NotNull().WithDefault(10)));

            SchemaBuilder.CreateTable(typeof(CommonSettingsRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(50))
                    .Column<bool>("Value"));

            SchemaBuilder.DropTable(typeof(CommonSettingsRecord).Name);

            SchemaBuilder.CreateTable(typeof(CommonSettingsRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<bool>("DoNotAcceptAnyNewCampaigns", column => column.NotNull().WithDefault(false))
                .Column<int>("ColoursPerPrint", column => column.NotNull().WithDefault(0)));

            var commonSettings = new CommonSettingsRecord();
            _commonSettingsRepository.Create(commonSettings);


            SchemaBuilder.CreateTable(typeof(PaymentSettingsRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Culture", c => c.WithLength(50))
                    .Column<int>("PaymentMethod"));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<int>("Environment", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("PublicKey", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("PrivateKey", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("MerchantId", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("ClientToken", c => c.Nullable()));



            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<int>("ProductMinimumGoal", c => c.NotNull().WithDefault(0)));

            SchemaBuilder.AlterTable(typeof(ProductRecord).Name,
                table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));


            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.DropColumn("ClientToken"));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("ClientToken", c => c.Unlimited()));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("Rejected", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("WhenApproved", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("WhenSentOut", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.DropColumn("WhenApproved"));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("WhenApproved", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("RequestUtcDate", column => column.NotNull())
                    .Column<bool>("EmailSent", column => column.NotNull().WithDefault(false))
                    .Column<DateTime>("EmailSentUtcDate"));

            SchemaBuilder.AlterTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .AddColumn<string>("Email", column => column.WithLength(100).NotNull()));

            SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
                table => table
                    .DropColumn("ColoursPerPrint"));

            SchemaBuilder.CreateTable(typeof(BringBackCampaignRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CampaignRecord_Id")
                    .Column<string>("Email", c => c.NotNull())
                );

            SchemaBuilder.CreateForeignKey("BringBackCampaign_Order", "BringBackCampaignRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(ArtRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<string>("FileName")
                );

            SchemaBuilder.AlterTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .DropColumn("EmailSent"));



            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name,
        table => table
        .AddColumn<int>("Currency_Id", column => column.NotNull().WithDefault(1)));

            SchemaBuilder.CreateForeignKey("CurrencyKey", "PayoutRecord", new[] { "Currency_Id" },
         "CurrencyRecord", new[] { "Id" });


            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
             table => table
        .DropColumn("AccountNumber"));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
              table => table
              .DropColumn("ContactNumber"));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
            table => table
            .AddColumn<string>("AccountNumber", column => column.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
            table => table
            .AddColumn<string>("ContactNumber", column => column.Nullable()));


            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
                           table => table
                               .DropColumn("AmountSize"));

            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
               table => table
               .AddColumn<double>("AmountSize"));

            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Country", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(LinkProductColorRecord).Name,
               table => table
                   .DropColumn("BaseCost"));

            SchemaBuilder.AlterTable(typeof(ProductRecord).Name,
               table => table
               .AddColumn<float>("BaseCost"));

            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("SecondProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("ThirdProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("FourthProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("FifthProductColorRecord_Id", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorSecond", "CampaignProductRecord",
                new[] { "SecondProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorThird", "CampaignProductRecord",
                new[] { "ThirdProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorFourth", "CampaignProductRecord",
                new[] { "FourthProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorFifth", "CampaignProductRecord",
                new[] { "FifthProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name, table => table.AddColumn<int>("ProductColorRecord_Id", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_ProductColor", "LinkOrderCampaignProductRecord",
                new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });

            SchemaBuilder.CreateTable(typeof(DeliverySettingRecord).Name,
               table => table
                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                   .Column<string>("State")
                   .Column<double>("DeliveryCost")
               );


            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.DropColumn("PaymentMethod"));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("CashDeliv", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("PayPal", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("Mol", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("CreditCard", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<string>("MerchantIdMol", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<string>("VerifyKey", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<bool>("IsArchived", c => c.NotNull().WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("BaseCampaignId", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("CampaignRecord_BaseCampaignId", "CampaignRecord",
                    new[] { "BaseCampaignId" }, "CampaignRecord", new[] { "Id" });


            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CntFrontColor"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CntBackColor"));

            SchemaBuilder.AlterTable(typeof(DeliverySettingRecord).Name, table => table.AddColumn<bool>("Enabled", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("CampaignCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(CurrencyRecord).Name, table => table.AddColumn<string>("CurrencyCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.AddColumn<string>("CostCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(CampaignCategoriesRecord).Name, table => table.AddColumn<string>("CategoriesCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
               table => table.AddColumn<string>("CashOnDeliveryAvailabilityMessage"));

            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("TeeyootUserCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
               table => table.DropColumn("CashOnDeliveryAvailabilityMessage"));

            SchemaBuilder.AlterTable(typeof(ArtRecord).Name, table => table.AddColumn<string>("ArtCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(ProductImageRecord).Name, table => table.AddColumn<string>("ProdImgCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(ProductHeadlineRecord).Name, table => table.AddColumn<string>("ProdHeadCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(ProductSizeRecord).Name, table => table.AddColumn<string>("ProdSizeCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(ProductGroupRecord).Name, table => table.AddColumn<string>("ProdGroupCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(FontRecord).Name, table => table.AddColumn<string>("FontCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(ProductColorRecord).Name, table => table.AddColumn<string>("ProdColorCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
            SchemaBuilder.AlterTable(typeof(SwatchRecord).Name, table => table.AddColumn<string>("SwatchCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name, table => table.AddColumn<string>("CommonCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            SchemaBuilder.AlterTable(typeof(DeliverySettingRecord).Name, table => table.AddColumn<string>("CommonCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            return 93;
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

            SchemaBuilder.CreateForeignKey("Order_Currency", "OrderRecord", new[] { "CurrencyRecord_Id" },
                "CurrencyRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_Order", "LinkOrderCampaignProductRecord",
                new[] { "OrderRecord_Id" }, "OrderRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_CampaignProduct", "LinkOrderCampaignProductRecord",
                new[] { "CampaignProductRecord_Id" }, "CampaignProductRecord", new[] { "Id" });

            return 4;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("Tags", c => c.Unlimited()));

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

            SchemaBuilder.CreateForeignKey("LinkProductGroup_Product", "LinkProductGroupRecord",
                new[] { "ProductRecord_Id" }, "ProductRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkProductGroup_ProductGroup", "LinkProductGroupRecord",
                new[] { "ProductGroupRecord_Id" }, "ProductGroupRecord", new[] { "Id" });

            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));

            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("StreetAddress"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PostalCode"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("PhoneNumber"));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("StreetAddress", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("PostalCode", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("PhoneNumber", c => c.WithLength(50)));


            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Created", c => c.NotNull()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Paid", c => c.Nullable()));

            SchemaBuilder.CreateTable(typeof(CampaignStatusRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(150))
                );

            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<int>("CampaignStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Campaign_Status", "CampaignRecord", new[] { "CampaignStatusRecord_Id" },
                "CampaignStatusRecord", new[] { "Id" });

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
            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord", new[] { "SizeId" },
                "ProductSizeRecord", new[] { "Id" });

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

            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_Campaign", "LinkCampaignAndCategoriesRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories",
                "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" },
                "CampaignCategoriesPartRecord", new[] { "Id" });

            return 14;
        }

        public int UpdateFrom14()
        {
            SchemaBuilder.DropForeignKey("LinkOrderCampaignProductRecord", "OrderProduct_Size");

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.DropColumn("SizeId"));

            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name,
                table => table.AddColumn<int>("ProductSizeRecord_Id"));

            SchemaBuilder.CreateForeignKey("OrderProduct_Size", "LinkOrderCampaignProductRecord",
                new[] { "ProductSizeRecord_Id" }, "ProductSizeRecord", new[] { "Id" });

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

            SchemaBuilder.CreateForeignKey("Store_TeeyootUser", "StoreRecord", new[] { "TeeyootUserId" },
                "TeeyootUserPartRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkStoreCampaignRecord_CampaignRecord", "LinkStoreCampaignRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_StoreRecord", "LinkStoreCampaignRecord",
                new[] { "StoreRecord_Id" }, "StoreRecord", new[] { "Id" });

            return 16;
        }

        public int UpdateFrom16()
        {
            SchemaBuilder.DropForeignKey("LinkCampaignAndCategoriesRecord",
                "LinkCampaignAndCategories_CampaignCategories");

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
            SchemaBuilder.CreateForeignKey("LinkCampaignAndCategories_CampaignCategories",
                "LinkCampaignAndCategoriesRecord", new[] { "CampaignCategoriesPartRecord_Id" }, "CampaignCategoriesRecord",
                new[] { "Id" });

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

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("Reserved", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<int>("OrderStatusRecord_Id"));

            SchemaBuilder.CreateForeignKey("Order_Status", "OrderRecord", new[] { "OrderStatusRecord_Id" },
                "OrderStatusRecord", new[] { "Id" });

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

            SchemaBuilder.CreateForeignKey("PayoutRecord_UserId", "PayoutRecord", new[] { "UserId" },
                "TeeyootUserPartRecord", new[] { "Id" });

            return 23;
        }

        public int UpdateFrom23()
        {
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Street", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Suit", c => c.WithLength(50)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("City", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("State", c => c.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Zip", c => c.WithLength(50)));

            return 24;
        }

        public int UpdateFrom24()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("OrderPublicId", c => c.NotNull().WithLength(50)));
            return 25;
        }

        public int UpdateFrom25()
        {
            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.DropColumn("CostOfMaterial"));
            return 26;
        }

        public int UpdateFrom26()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(false)));
            return 27;
        }

        public int UpdateFrom27()
        {
            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name,
                table => table.AddColumn<string>("Status", c => c.NotNull().WithLength(50)));
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

        public int UpdateFrom29()
        {
            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
                table => table.AddColumn<int>("Userid", c => c.NotNull()));

            SchemaBuilder.CreateForeignKey("PromotionRecord_UserId", "PromotionRecord", new[] { "UserId" },
                "TeeyootUserPartRecord", new[] { "Id" });

            return 30;
        }

        public int UpdateFrom30()
        {
            SchemaBuilder.CreateTable(typeof(OrderHistoryRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("EventDate")
                    .Column<string>("Event", c => c.Unlimited())
                    .Column<int>("OrderRecord_Id")
                );

            SchemaBuilder.CreateForeignKey("OrderHistory_Order", "OrderHistoryRecord", new[] { "OrderRecord_Id" },
                "OrderRecord", new[] { "Id" });

            return 31;
        }

        public int UpdateFrom31()
        {
            SchemaBuilder.CreateTable(typeof(PaymentInformationRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("AccountNumber")
                    .Column<string>("BankName")
                    .Column<int>("ContactNumber")
                    .Column<string>("MessAdmin")
                    .Column<int>("TranzactionId")
                );

            SchemaBuilder.CreateForeignKey("Tranzaction_Payout_Id", "PaymentInformationRecord", new[] { "TranzactionId" },
                "PayoutRecord", new[] { "Id" });

            return 32;
        }

        public int UpdateFrom32()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<double>("Promotion"));

            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.AddColumn<double>("TotalPriceWithPromo"));

            return 33;
        }

        public int UpdateFrom33()
        {
            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
                table => table.AddColumn<string>("AccountHolderName"));
            return 34;
        }

        public int UpdateFrom34()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsActive", c => c.NotNull().WithDefault(true)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsApproved", c => c.NotNull().WithDefault(false)));

            return 35;
        }

        public int UpdateFrom35()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<string>("TranzactionId", c => c.Nullable()));
            return 36;
        }

        public int UpdateFrom36()
        {
            SchemaBuilder.CreateTable(typeof(MessageRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("UserId")
                    .Column<string>("Text")
                    .Column<string>("From")
                    .Column<DateTime>("SendDate")
                );

            SchemaBuilder.CreateForeignKey("Message_User_Id", "MessageRecord", new[] { "UserId" }, "TeeyootUserPartRecord",
                new[] { "Id" });

            return 37;
        }

        public int UpdateFrom37()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("Delete", c => c.WithDefault(null)));
            return 38;
        }

        public int UpdateFrom38()
        {
            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.DropColumn("Text"));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Text", c => c.Unlimited()));

            return 39;
        }

        public int UpdateFrom39()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.DropColumn("Delete"));

            return 40;
        }

        public int UpdateFrom40()
        {
            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.DropColumn("From"));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Sender", c => c.WithLength(50)));

            return 41;
        }

        public int UpdateFrom41()
        {

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name, table => table.AddColumn<int>("CampaignId"));

            SchemaBuilder.CreateForeignKey("Message_Campaign_Id", "MessageRecord", new[] { "CampaignId" },
                "CampaignRecord", new[] { "Id" });

            return 42;
        }

        public int UpdateFrom42()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));

            return 43;
        }

        public int UpdateFrom43()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("IsPrivate", c => c.NotNull().WithDefault(false)));

            return 44;
        }

        public int UpdateFrom44()
        {
            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<bool>("IsApprowed", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(MessageRecord).Name,
                table => table.AddColumn<string>("Subject", c => c.WithLength(50)));

            return 45;
        }

        public int UpdateFrom45()
        {
            SchemaBuilder.AlterTable(typeof(LinkProductSizeRecord).Name,
                table => table.AddColumn<float>("SizeCost", c => c.NotNull().WithDefault(float.Parse("0"))));

            return 46;
        }

        public int UpdateFrom46()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<bool>("ProfitPaid", c => c.NotNull().WithDefault(false)));

            return 47;
        }

        public int UpdateFrom47()
        {
            SchemaBuilder.CreateTable(typeof(MailChimpSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("ApiKey", c => c.WithLength(50))
                    .Column<string>("Culture", c => c.WithLength(50)));

            return 48;
        }

        public int UpdateFrom48()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<string>("CampaignProfit", c => c.NotNull().WithDefault(string.Empty)));

            return 49;
        }

        public int UpdateFrom49()
        {
            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name,
                table => table.AddColumn<int>("SalesGoal", c => c.NotNull().WithDefault(500)));

            return 50;
        }

        public int UpdateFrom50()
        {
            SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name,
                table => table.AddColumn<int>("MaxColors", c => c.NotNull().WithDefault(10)));

            return 51;
        }

        public int UpdateFrom51()
        {
            SchemaBuilder.CreateTable(typeof(CommonSettingsRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", c => c.WithLength(50))
                    .Column<bool>("Value"));

            return 52;
        }

        public int UpdateFrom52()
        {
            SchemaBuilder.DropTable(typeof(CommonSettingsRecord).Name);

            SchemaBuilder.CreateTable(typeof(CommonSettingsRecord).Name, table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<bool>("DoNotAcceptAnyNewCampaigns", column => column.NotNull().WithDefault(false))
                .Column<int>("ColoursPerPrint", column => column.NotNull().WithDefault(0)));

            var commonSettings = new CommonSettingsRecord();
            _commonSettingsRepository.Create(commonSettings);

            return 53;
        }

        public int UpdateFrom53()
        {
            SchemaBuilder.CreateTable(typeof(PaymentSettingsRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Culture", c => c.WithLength(50))
                    .Column<int>("PaymentMethod"));

            return 54;
        }

        public int UpdateFrom54()
        {
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<int>("Environment", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("PublicKey", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("PrivateKey", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("MerchantId", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("ClientToken", c => c.Nullable()));

            return 55;
        }

        public int UpdateFrom55()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<int>("ProductMinimumGoal", c => c.NotNull().WithDefault(0)));

            return 56;
        }

        public int UpdateFrom56()
        {
            SchemaBuilder.AlterTable(typeof(ProductRecord).Name,
                table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));
            return 57;
        }

        public int UpdateFrom57()
        {
            //SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AlterColumn("ClientToken", c => c.Unlimited()));
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.DropColumn("ClientToken"));
            return 58;
        }

        public int UpdateFrom58()
        {
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name,
                table => table.AddColumn<string>("ClientToken", c => c.Unlimited()));
            return 59;
        }

        public int UpdateFrom59()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<bool>("Rejected", c => c.NotNull().WithDefault(false)));

            return 60;
        }


        public int UpdateFrom60()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("WhenApproved", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name,
                table => table.AddColumn<DateTime>("WhenSentOut", c => c.Nullable()));

            return 61;
        }

        public int UpdateFrom61()
        {
            SchemaBuilder.AlterTable(typeof(OrderRecord).Name, table => table.DropColumn("WhenApproved"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name,
                table => table.AddColumn<DateTime>("WhenApproved", c => c.Nullable()));

            return 62;
        }

        public int UpdateFrom62()
        {
            SchemaBuilder.CreateTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<DateTime>("RequestUtcDate", column => column.NotNull())
                    .Column<bool>("EmailSent", column => column.NotNull().WithDefault(false))
                    .Column<DateTime>("EmailSentUtcDate"));

            return 63;
        }

        public int UpdateFrom63()
        {
            SchemaBuilder.AlterTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .AddColumn<string>("Email", column => column.WithLength(100)));

            SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
                table => table
                    .DropColumn("ColoursPerPrint"));

            return 64;
        }

        public int UpdateFrom64()
        {
            SchemaBuilder.CreateTable(typeof(BringBackCampaignRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CampaignRecord_Id")
                    .Column<string>("Email", c => c.NotNull())
                );

            SchemaBuilder.CreateForeignKey("BringBackCampaign_Order", "BringBackCampaignRecord",
                new[] { "CampaignRecord_Id" }, "CampaignRecord", new[] { "Id" });

            return 65;
        }

        public int UpdateFrom65()
        {
            SchemaBuilder.CreateTable(typeof(ArtRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name")
                    .Column<string>("FileName")
                );

            return 66;
        }

        public int UpdateFrom66()
        {
            SchemaBuilder.AlterTable(typeof(CheckoutCampaignRequest).Name,
                table => table
                    .DropColumn("EmailSent"));

            return 67;
        }

        public int UpdateFrom67()
        {
            SchemaBuilder.AlterTable(typeof(PayoutRecord).Name,
        table => table
        .AddColumn<int>("Currency_Id", column => column.NotNull().WithDefault(1)));

            SchemaBuilder.CreateForeignKey("CurrencyKey", "PayoutRecord", new[] { "Currency_Id" },
         "CurrencyRecord", new[] { "Id" });
            return 68;
        }

        public int UpdateFrom68()
        {
            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
                table => table
                    .DropColumn("AccountNumber"));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
              table => table
              .DropColumn("ContactNumber"));

            return 69;
        }


        public int UpdateFrom69()
        {
            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
            table => table
            .AddColumn<string>("AccountNumber", column => column.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentInformationRecord).Name,
            table => table
            .AddColumn<string>("ContactNumber", column => column.Nullable()));

            return 70;
        }

        public int UpdateFrom70()
        {
            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
               table => table
                   .DropColumn("AmountSize"));

            SchemaBuilder.AlterTable(typeof(PromotionRecord).Name,
               table => table
               .AddColumn<double>("AmountSize"));

            return 71;
        }

        public int UpdateFrom71()
        {
            SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("Country", c => c.WithLength(100)));
            return 72;
        }

        public int UpdateFrom72()
        {
            SchemaBuilder.AlterTable(typeof(LinkProductColorRecord).Name,
               table => table
                   .DropColumn("BaseCost"));

            SchemaBuilder.AlterTable(typeof(ProductRecord).Name,
               table => table
               .AddColumn<float>("BaseCost"));

            return 73;
        }

        public int UpdateFrom73()
        {
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("SecondProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("ThirdProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("FourthProductColorRecord_Id", c => c.Nullable()));
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<int>("FifthProductColorRecord_Id", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorSecond", "CampaignProductRecord",
                new[] { "SecondProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorThird", "CampaignProductRecord",
                new[] { "ThirdProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorFourth", "CampaignProductRecord",
                new[] { "FourthProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });
            SchemaBuilder.CreateForeignKey("CampaignProduct_ProductColorFifth", "CampaignProductRecord",
                new[] { "FifthProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });

            return 74;
        }

        public int UpdateFrom74()
        {
            SchemaBuilder.AlterTable(typeof(LinkOrderCampaignProductRecord).Name, table => table.AddColumn<int>("ProductColorRecord_Id", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("LinkOrderCampaignProduct_ProductColor", "LinkOrderCampaignProductRecord",
                new[] { "ProductColorRecord_Id" }, "ProductColorRecord", new[] { "Id" });

            return 75;
        }

        public int UpdateFrom75()
        {
            SchemaBuilder.CreateTable(typeof(DeliverySettingRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("State")
                    .Column<double>("DeliveryCost", column => column.NotNull().WithDefault(0.0))
                );

            return 76;
        }

        public int UpdateFrom76()
        {
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.DropColumn("PaymentMethod"));

            return 77;
        }

        public int UpdateFrom77()
        {
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("CashDeliv", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("PayPal", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("Mol", c => c.NotNull().WithDefault(false)));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<bool>("CreditCard", c => c.NotNull().WithDefault(false)));

            return 78;
        }

        public int UpdateFrom78()
        {
            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<string>("MerchantIdMol", c => c.Nullable()));

            SchemaBuilder.AlterTable(typeof(PaymentSettingsRecord).Name, table => table.AddColumn<string>("VerifyKey",  c => c.Nullable()));

            return 79;
        }


        public int UpdateFrom79()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<bool>("IsArchived", c => c.NotNull().WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("BaseCampaignId", c => c.Nullable()));

            SchemaBuilder.CreateForeignKey("CampaignRecord_BaseCampaignId", "CampaignRecord",
                    new[] { "BaseCampaignId" }, "CampaignRecord", new[] { "Id" });

            return 80;
        }

        public int UpdateFrom80()
        {
            SchemaBuilder.AlterTable(typeof(CampaignProductRecord).Name, table => table.AddColumn<DateTime>("WhenDeleted", c => c.Nullable()));

            return 81;
        }

        public int UpdateFrom81()
        {
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CntFrontColor"));
            SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<int>("CntBackColor"));


            return 82;
        }

       public int UpdateFrom82()
        {
            SchemaBuilder.AlterTable(typeof(DeliverySettingRecord).Name, table => table.AddColumn<bool>("Enabled", c => c.NotNull().WithDefault(false)));
            return 83;
        }

       public int UpdateFrom83()
       {
           SchemaBuilder.AlterTable(typeof(CampaignRecord).Name, table => table.AddColumn<string>("CampaignCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 84;
       }

       public int UpdateFrom84()
       {
           SchemaBuilder.AlterTable(typeof(CurrencyRecord).Name, table => table.AddColumn<string>("CurrencyCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 85;
       }

       public int UpdateFrom85()
       {
           SchemaBuilder.AlterTable(typeof(TShirtCostRecord).Name, table => table.AddColumn<string>("CostCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(CampaignCategoriesRecord).Name, table => table.AddColumn<string>("CategoriesCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 86;
       }

       public int UpdateFrom86()
       {
           SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
               table => table.AddColumn<string>("CashOnDeliveryAvailabilityMessage"));

           return 87;
       }

       public int UpdateFrom87()
       {
           SchemaBuilder.AlterTable(typeof(TeeyootUserPartRecord).Name,
                table => table.AddColumn<string>("TeeyootUserCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 88;
       }

       public int UpdateFrom88()
       {
           SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name,
               table => table.DropColumn("CashOnDeliveryAvailabilityMessage"));

           return 89;
       }

       public int UpdateFrom89()
       {
           SchemaBuilder.AlterTable(typeof(ArtRecord).Name, table => table.AddColumn<string>("ArtCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(ProductImageRecord).Name, table => table.AddColumn<string>("ProdImgCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(ProductHeadlineRecord).Name, table => table.AddColumn<string>("ProdHeadCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(ProductSizeRecord).Name, table => table.AddColumn<string>("ProdSizeCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(ProductGroupRecord).Name, table => table.AddColumn<string>("ProdGroupCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(FontRecord).Name, table => table.AddColumn<string>("FontCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 90;
       }

       public int UpdateFrom90()
       {
           SchemaBuilder.AlterTable(typeof(ProductColorRecord).Name, table => table.AddColumn<string>("ProdColorCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
           SchemaBuilder.AlterTable(typeof(SwatchRecord).Name, table => table.AddColumn<string>("SwatchCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));
        
           return 91;
       }
     
        public int UpdateFrom91()
       {
           SchemaBuilder.AlterTable(typeof(CommonSettingsRecord).Name, table => table.AddColumn<string>("CommonCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

           return 92;
       }

        public int UpdateFrom92()
        {
            SchemaBuilder.AlterTable(typeof(DeliverySettingRecord).Name, table => table.AddColumn<string>("CommonCulture", c => c.NotNull().WithDefault("en-MY").WithLength(10)));

            return 93;
        }
    }
}