using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using System;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services
{
    public interface IDeliverySettingsService : IDependency
    {
        void DeleteSetting(int id);

        void AddSetting(string state, double deliveryCost);

        void EditSetting(EditDeliverySettingViewModel newSetting);

        void UpdateSetting(DeliverySettingRecord setting);

        IQueryable<DeliverySettingRecord> GetAllSettings();

        DeliverySettingRecord GetSettingById(int settingId);

    }

    
}
