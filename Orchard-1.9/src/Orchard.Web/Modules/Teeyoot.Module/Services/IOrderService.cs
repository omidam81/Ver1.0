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
        OrderRecord GetCOrderById(int id);

        
    }
}
