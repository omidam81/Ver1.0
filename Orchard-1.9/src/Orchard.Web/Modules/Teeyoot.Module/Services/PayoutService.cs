using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;

namespace Teeyoot.Module.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IRepository<PayoutRecord> _payoutRepository;

        public PayoutService(IRepository<PayoutRecord> payoutRepository)
        {
            _payoutRepository = payoutRepository;
        }

        public IQueryable<PayoutRecord> GetAllPayouts()
        {
            return _payoutRepository.Table;
        }

        public void AddPayout(PayoutRecord payout)
        {
            _payoutRepository.Create(payout);

        }

        public void UpdatePayout(PayoutRecord payout)
        {
            _payoutRepository.Update(payout);

        }
    }
}