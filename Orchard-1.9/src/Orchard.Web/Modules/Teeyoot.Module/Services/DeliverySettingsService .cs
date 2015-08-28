using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using System;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services
{
    public class DeliverySettingsService : IDeliverySettingsService
    {
        private readonly IRepository<DeliverySettingRecord> _deliverySettingsRepository;

        public DeliverySettingsService(IRepository<DeliverySettingRecord> deliverySettingsRepository)
        {
            _deliverySettingsRepository = deliverySettingsRepository;
        }

        public void DeleteSetting(int id)
        {
            _deliverySettingsRepository.Delete(_deliverySettingsRepository.Get(id));
        }

        public void AddSetting(string state, double deliveryCost)
        {
           
            try
            {
                var newSetting = new DeliverySettingRecord()
                {
                    State = state,
                    DeliveryCost = deliveryCost

                };
                _deliverySettingsRepository.Create(newSetting);
            }
            catch
            {
                throw;
            }
        }

        public DeliverySettingRecord GetSettingById(int settingId)
        {
            return _deliverySettingsRepository.Get(f => f.Id == settingId);
        }

        public IQueryable<DeliverySettingRecord> GetAllSettings()
        {
            return _deliverySettingsRepository.Table;
        }


        public void EditSetting(EditDeliverySettingViewModel newSetting)
        {
            DeliverySettingRecord setting = _deliverySettingsRepository.Get(f => f.Id == newSetting.Id);
            setting.State = newSetting.State;
            setting.DeliveryCost = newSetting.DeliveryCost;
            _deliverySettingsRepository.Update(setting);

        }

    }
}