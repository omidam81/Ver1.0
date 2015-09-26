using System.Web;

namespace Teeyoot.Localization.LocalizationStorage
{
    public class HttpLocalizationInfoStorageContainer : ILocalizationInfoStorageContainer
    {
        private const string LocalizationInfoKey = "LocalizationInfo";

        public ILocalizationInfo GetCurrentLocalizationInfo()
        {
            ILocalizationInfo localizationInfo = null;

            if (HttpContext.Current.Items.Contains(LocalizationInfoKey))
                localizationInfo = (ILocalizationInfo) HttpContext.Current.Items[LocalizationInfoKey];

            return localizationInfo;
        }

        public void Store(ILocalizationInfo localizationInfo)
        {
            if (HttpContext.Current.Items.Contains(LocalizationInfoKey))
                HttpContext.Current.Items[LocalizationInfoKey] = localizationInfo;
            else
                HttpContext.Current.Items.Add(LocalizationInfoKey, localizationInfo);
        }
    }
}
