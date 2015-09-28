using System.Web;

namespace Teeyoot.Localization.LocalizationStorage
{
    public static class LocalizationInfoStorageContainerFactory
    {
        private static ILocalizationInfoStorageContainer _localizationInfoStorageContainer;

        public static ILocalizationInfoStorageContainer GetStorageContainer()
        {
            if (_localizationInfoStorageContainer != null) 
                return _localizationInfoStorageContainer;

            if (HttpContext.Current == null)
                _localizationInfoStorageContainer = new ThreadLocalizationInfoStorageContainer();
            else
                _localizationInfoStorageContainer = new HttpLocalizationInfoStorageContainer();

            return _localizationInfoStorageContainer;
        }
    }
}
