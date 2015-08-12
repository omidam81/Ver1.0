using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface IPaymentSettingsService : IDependency
    {
        IQueryable<PaymentSettingsRecord> GetAllSettigns();

        void AddSettings(PaymentSettingsRecord payment);

        void UpdateSettings(PaymentSettingsRecord payment);

    }
}
