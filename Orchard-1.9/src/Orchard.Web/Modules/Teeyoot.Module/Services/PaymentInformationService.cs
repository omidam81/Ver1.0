using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Services
{
    public class PaymentInformationService : IPaymentInformationService
    {
        private readonly IRepository<PaymentInformationRecord> _paymentInfRepository;

        public PaymentInformationService(IRepository<PaymentInformationRecord> paymentInfRepository)
        {
            _paymentInfRepository = paymentInfRepository;
        }

        public IQueryable<PaymentInformationRecord> GetAllPayments()
        {
            return _paymentInfRepository.Table;
        }

        public void AddPayment(PaymentInformationRecord payment)
        {
            _paymentInfRepository.Create(payment);
        }

    }
}