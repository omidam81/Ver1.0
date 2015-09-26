using System.Web;

namespace Teeyoot.Localization.LocalizationStorage
{
    public static class LocalizationInfoStorageContainerFactory
    {
        public static ILocalizationInfoStorageContainer GetStorageContainer()
        {
            if (HttpContext.Current != null)
                return new HttpLocalizationInfoStorageContainer();

            return new ThreadLocalizationInfoStorageContainer();
        }
    }
}
