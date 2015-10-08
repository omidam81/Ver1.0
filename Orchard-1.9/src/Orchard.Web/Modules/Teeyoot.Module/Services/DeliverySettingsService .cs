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
        private readonly IRepository<CountryRecord> _countryRepository;
        private readonly IWorkContextAccessor _workContextAccessor;
        

        public DeliverySettingsService(IRepository<DeliverySettingRecord> deliverySettingsRepository, 
                                        IRepository<CountryRecord> countryRepository,
                                        IWorkContextAccessor workContextAccessor)
        {
            _deliverySettingsRepository = deliverySettingsRepository;
            _countryRepository = countryRepository;
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
        public void AddSetting(string state, double postageCost, double codCost, int countryId,
                //todo: (aut:juiceek) drop this param        
                string culture)
        {
            try
            {
                var newRecord = new DeliverySettingRecord()
                {
                    State = state,
                    //DeliveryCost = deliveryCost,
                    DeliveryCulture = culture,
                    Country =  _countryRepository.Get(countryId),
                    PostageCost = postageCost,
                    CodCost = codCost
                };
                _deliverySettingsRepository.Create(newRecord);
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


        public void EditSetting(EditDeliverySettingViewModel viewModel)
        {
            DeliverySettingRecord record = _deliverySettingsRepository.Get(f => f.Id == viewModel.Id);
            record.State = viewModel.State;
            // todo: (auth:juiceek) drop this 
            //setting.DeliveryCost = newSetting.DeliveryCost;
            record.Country = _countryRepository.Get(viewModel.CountryId);
            record.PostageCost = viewModel.PostageCost;
            record.CodCost = viewModel.CodCost;

            _deliverySettingsRepository.Update(record);

        }



    }
}