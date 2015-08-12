using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Services
{
    public class PaymentSettingsService : IPaymentSettingsService
    {
        private readonly IRepository<PaymentSettingsRecord> _paymentSettingsRepository;

        public PaymentSettingsService(IRepository<PaymentSettingsRecord> paymentSettingsRepository) 
        {
            _paymentSettingsRepository = paymentSettingsRepository;
        }

        public IQueryable<PaymentSettingsRecord> GetAllSettigns()
        {
            return _paymentSettingsRepository.Table;
        }

        public void AddSettings(PaymentSettingsRecord payment)
        {
            _paymentSettingsRepository.Create(payment);
        }

        public void UpdateSettings(PaymentSettingsRecord payment)
        {
            _paymentSettingsRepository.Update(payment);
        }
    }
}