using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Teeyoot.Module
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

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
                        .Action("Index", "Home", new { area = "Teeyoot.Module" })
                    )

                    .Add(subItem => subItem
                        .Caption(T("FAQ"))
                        .Position("2.2")
                        .Action("Index", "FaqAdmin", new { area = "Teeyoot.FAQ" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Mail Chimp Settings"))
                        .Position("2.3")
                        .Action("Index", "AdminMessage", new { area = "Teeyoot.Messaging" })
                    )
                    .Add(subItem => subItem
                        .Caption(T("Categories"))
                        .Position("2.4")
                        .Action("Index", "AdminSearch", new { area = "Teeyoot.Search" })
                    )
                );
        }
    }
}