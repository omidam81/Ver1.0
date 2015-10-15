using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface IDeliverySettingsService : IDependency
    {
        void DeleteSetting(int id);

        void AddSetting(
            string state,
            double postageCost,
            double codCost,
            int countryId,
            //todo: (auth:juiceek) drop this param
            string culture);

        void EditSetting(EditDeliverySettingViewModel newSetting);
        void UpdateSetting(DeliverySettingRecord setting);
        IQueryable<DeliverySettingRecord> GetAllSettings(int countryId);
        DeliverySettingRecord GetSettingById(int settingId);
    }
}
