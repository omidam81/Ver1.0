using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using System;
using Teeyoot.Module.ViewModels;
using Orchard;

namespace Teeyoot.Module.Services
{
    public class DeliverySettingsService : IDeliverySettingsService
    {
        private readonly IRepository<DeliverySettingRecord> _deliverySettingsRepository;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DeliverySettingsService(IRepository<DeliverySettingRecord> deliverySettingsRepository, IWorkContextAccessor workContextAccessor)
        {
            _deliverySettingsRepository = deliverySettingsRepository;
            _workContextAccessor = workContextAccessor;
            
        }

        public void DeleteSetting(int id)
        {
            _deliverySettingsRepository.Delete(_deliverySettingsRepository.Get(id));
        }


        public void UpdateSetting(DeliverySettingRecord setting)
        {
            _deliverySettingsRepository.Update(setting);

        }
        public void AddSetting(string state, double deliveryCost, string culture)
        {
           
            try
            {
                var newSetting = new DeliverySettingRecord()
                {
                    State = state,
                    DeliveryCost = deliveryCost,
                    DeliveryCulture = culture

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
            var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            var settings = _deliverySettingsRepository.Table.Where(s => s.DeliveryCulture == cultureUsed);

            if (settings.FirstOrDefault() == null)
            {
                _deliverySettingsRepository.Create(new DeliverySettingRecord() { DeliveryCost = 0, State = "Default", DeliveryCulture = cultureUsed, Enabled = true });
            }
            return _deliverySettingsRepository.Table.Where(s=>s.DeliveryCulture == cultureUsed);
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