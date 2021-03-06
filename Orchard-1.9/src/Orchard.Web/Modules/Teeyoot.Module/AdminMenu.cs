﻿using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Teeyoot.Module
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.AddImageSet("teeyoot")
                .Add(item => item

                    .Caption(T("Teeyoot"))
                    .Position("2")
                    .LinkToFirstChild(true)

                    .Add(subItem => subItem
                        .Caption(T("Teeyoot"))
                        .Position("2.1")
                    //.Action("Index", "Home", new { area = "Teeyoot.Module" })
                    )
                     .Add(subItem => subItem
                        .Caption(T("Campaigns"))
                        .Position("2.6")
                        .Action("Index", "AdminFeaturedCampaigns", new { area = "Teeyoot.FeaturedCampaigns" })
                        .Add(T("Status"),
                            i => i.Action("Index", "AdminFeaturedCampaigns", new { area = "Teeyoot.FeaturedCampaigns" }).LocalNav())
                        .Add(T("Export"),
                            i => i.Action("Index", "AdminExportPrints", new { area = "Teeyoot.FeaturedCampaigns" }).LocalNav())
                        .Add(T("Details"),
                            i => i.Action("Index", "AdminCampaignsSettings", new { area = "Teeyoot.FeaturedCampaigns" }).LocalNav())
                    )
                     .Add(subItem => subItem
                        .Caption(T("Orders"))
                        .Position("3")
                        .Action("Index", "Home", new { area = "Teeyoot.Orders" })
                    )
                     .Add(subItem => subItem
                        .Caption(T("Payouts"))
                        .Position("2.8")
                        .Action("Payouts", "Tranzaction", new { area = "Teeyoot.Payouts" })
                        .Add(T("Payouts"),
                            i => i.Action("Payouts", "Tranzaction", new { area = "Teeyoot.Payouts" }).LocalNav())
                        .Add(T("Order profits"),
                            i => i.Action("Index", "Tranzaction", new { area = "Teeyoot.Payouts" }).LocalNav())
                    )
                    .Add(subItem => subItem
                        .Caption(T("Messages"))
                        .Position("2.9")
                        .Action("Index", "AdminMessageContent", new { area = "Teeyoot.Messaging" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("FAQ"))
                        .Position("2.2")
                        .Action("Index", "FaqAdmin", new { area = "Teeyoot.FAQ" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Categories"))
                        .Position("2.4")
                        .Action("Index", "AdminSearch", new { area = "Teeyoot.Search" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Product Settings"))
                        .Position("2.5")
                        .Action("Index", "AdminWizard", new { area = "Teeyoot.WizardSettings" })
                        .Add(T("Artworks"),
                            i => i.Action("Index", "Artwork", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Styles"),
                            i => i.Action("Index", "Product", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Types"),
                            i => i.Action("Index", "ProductStyle", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Headlines"),
                            i => i.Action("Index", "ProductHeadline", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Sizes"),
                            i => i.Action("Index", "ProductSize", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Colors"),
                            i => i.Action("Index", "Colour", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("Fonts"),
                            i => i.Action("FontList", "AdminWizard", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                    )
                    .Add(subItem => subItem
                        .Caption(T("Cost Calculator"))
                        .Position("2.7")
                        .Action("Index", "AdminCost", new { area = "Teeyoot.Module" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Payment Settings"))
                        .Position("3.1")
                        .Action("Index", "Payment", new { area = "Teeyoot.PaymentSettings" })
                        )
                    .Add(subItem => subItem
                        .Caption(T("Relaunch Requests"))
                        .Position("3.2")
                        .Action("Index", "AdminRelaunchCamp", new { area = "Teeyoot.Module" })
                        )
                    .Add(subItem => subItem
                        .Caption(T("Promotions"))
                        .Position("3.2")
                        .Action("Index", "AdminPromotions", new { area = "Teeyoot.Module" })
                        )
                    .Add(subItem => subItem
                        .Caption(T("Text translation"))
                        .Position("3.4")
                        .Action("Index", "AdminTranslationText", new { area = "Teeyoot.Module" })
                        )
                    .Add(subItem => subItem
                        .Caption(T("Mandrill Settings"))
                        .Position("2.3")
                        .Action("Index", "AdminMessage", new { area = "Teeyoot.Messaging" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Common Settings"))
                        .Action("Index", "AdminCommonSettings", new { area = "Teeyoot.Module" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Delivery Settings"))
                        .Position("2.8")
                        .Action("Index", "DeliverySettings", new { area = "Teeyoot.WizardSettings" })
                        .Add(T("Domestic"),
                            i => i.Action("Index", "DeliverySettings", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                        .Add(T("International"),
                            i => i.Action("Index", "DeliveryInternationalSettings", new { area = "Teeyoot.WizardSettings" }).LocalNav())
                    )
                    .Add(subItem => subItem
                        .Caption(T("Users"))
                        .Position("2.8")
                        .Action("Index", "AdminUser", new { area = "Teeyoot.Module" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Currency Converter"))
                        .Position("2.9")
                        .Action("Index", "AdminCurrenciesConvertationTable", new { area = "Teeyoot.Module" })
                        .Add(T("Convertation Table"),
                            i => i.Action("Index", "AdminCurrenciesConvertationTable", new { area = "Teeyoot.Module" }).LocalNav())
                        .Add(T("Currencies"),
                            i => i.Action("Index", "AdminCurrencies", new { area = "Teeyoot.Module" }).LocalNav())
                    )
                    .Add(subItem => subItem
                        .Caption(T("Countries"))
                        .Position("2.8")
                        .Action("Index", "AdminCountries", new { area = "Teeyoot.Module" })
                    )
                );
        }
    }
}