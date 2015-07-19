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

        public OrderRecord GetOrderById(int id)
        {
            //return _repository.Table.FirstOrDefault(r => r.Id == id);

            OrderRecord order = new OrderRecord() { Id = id };
            return order;
  
        }

        public void UpdateOrder(OrderRecord order)
        {
           // _repository.Update(order);    
        }

        public OrderRecord CreateOrder(OrderRecord order)
        {
            //return _repository.Create(order);
            order.Id = 12;
            return order;
        }
    }
}
