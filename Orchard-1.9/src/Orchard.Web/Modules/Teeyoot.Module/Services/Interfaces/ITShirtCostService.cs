using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services.Interfaces
{
    public interface ITShirtCostService : IDependency
    {
        TShirtCostRecord GetCost(string culture);

        bool UpdateCost(TShirtCostRecord cost);

        bool InsertCost(TShirtCostRecord cost);
    }
}
