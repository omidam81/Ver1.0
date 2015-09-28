using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;
using Orchard.ContentManagement;
using Orchard;

namespace Teeyoot.Module.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IRepository<PayoutRecord> _payoutRepository;
        private readonly IContentManager _contentManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<TeeyootUserPartRecord> _teeyootUserRepos;

        public PayoutService(IRepository<PayoutRecord> payoutRepository, IContentManager contentManager,
            IWorkContextAccessor workContextAccessor, IRepository<TeeyootUserPartRecord> teeyootUserRepos)
        {
            _payoutRepository = payoutRepository;
            _contentManager = contentManager;
            _workContextAccessor = workContextAccessor;
            _teeyootUserRepos = teeyootUserRepos;
        }

        public IQueryable<PayoutRecord> GetAllPayouts()
        {
            //var culture = _workContextAccessor.GetContext().CurrentCulture.Trim();
            //string cultureUsed = culture == "en-SG" ? "en-SG" : (culture == "id-ID" ? "id-ID" : "en-MY");
            //var users = _teeyootUserRepos.Table.Where(c => c.TeeyootUserCulture == cultureUsed).Select(c => c.Id);
            var users = _teeyootUserRepos.Table.Select(c => c.Id);
            return _payoutRepository.Table.Where(p => users.Contains(p.UserId));
        }

        public void AddPayout(PayoutRecord payout)
        {
            _payoutRepository.Create(payout);

        }

        public void UpdatePayout(PayoutRecord payout)
        {
            _payoutRepository.Update(payout);

        }

        public bool DeletePayoutByOrderPublicId(string publicId)
        {
            var payout = _payoutRepository.Table.Where(p => p.Event == publicId && p.IsPlus == true).FirstOrDefault();
            if (payout != null)
            {
                _payoutRepository.Delete(payout);
                _payoutRepository.Flush();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}