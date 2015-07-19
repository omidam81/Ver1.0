using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface IOrderService : IDependency
    {
        OrderRecord GetOrderById(int id);

        void UpdateOrder(OrderRecord order);

        OrderRecord CreateOrder(OrderRecord order);
    }
}
