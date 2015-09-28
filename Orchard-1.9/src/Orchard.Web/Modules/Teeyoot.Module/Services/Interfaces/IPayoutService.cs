using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface IPayoutService : IDependency
    {
        IQueryable<PayoutRecord> GetAllPayouts();
        void AddPayout(PayoutRecord payout );
        void UpdatePayout(PayoutRecord payout);
        bool DeletePayoutByOrderPublicId(string publicId);
        
    }
}
