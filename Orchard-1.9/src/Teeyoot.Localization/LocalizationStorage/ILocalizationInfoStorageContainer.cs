namespace Teeyoot.Localization.LocalizationStorage
{
    public interface ILocalizationInfoStorageContainer
    {
        ILocalizationInfo GetCurrentLocalizationInfo();
        void Store(ILocalizationInfo localizationInfo);
    }
}
