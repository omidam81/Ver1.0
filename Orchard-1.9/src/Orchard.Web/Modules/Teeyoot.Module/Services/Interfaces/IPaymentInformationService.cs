using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
   public interface IPaymentInformationService : IDependency
    {
       IQueryable<PaymentInformationRecord> GetAllPayments();
       void AddPayment(PaymentInformationRecord payment);
    }
}
