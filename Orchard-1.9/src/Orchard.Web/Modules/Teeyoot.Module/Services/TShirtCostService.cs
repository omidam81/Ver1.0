using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Services.Interfaces;
using Teeyoot.Module.Models;
using Orchard.Data;

namespace Teeyoot.Module.Services
{
    public class TShirtCostService : ITShirtCostService
    {
        private readonly IRepository<TShirtCostRecord> _costRepository;

        public TShirtCostService(IRepository<TShirtCostRecord> costRepository)
        {
            _costRepository = costRepository;
        }

        public TShirtCostRecord GetCost(string culture)
        {
            return _costRepository.Table.Where(c => c.CostCulture == culture).FirstOrDefault();
        }

        public bool UpdateCost(TShirtCostRecord cost)
        {
            try
            {
                _costRepository.Update(cost);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertCost(TShirtCostRecord cost)
        {
            try
            {
                _costRepository.Create(cost);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}