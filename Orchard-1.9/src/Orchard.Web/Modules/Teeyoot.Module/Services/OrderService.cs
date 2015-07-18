using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<OrderRecord> _repository;

        public OrderService(IRepository<OrderRecord> repository)
	    {
            _repository = repository;
	    }

        public OrderRecord GetCOrderById(int id)
        {
            return _repository.Table.FirstOrDefault(r => r.Id == id);
        }
    }
}