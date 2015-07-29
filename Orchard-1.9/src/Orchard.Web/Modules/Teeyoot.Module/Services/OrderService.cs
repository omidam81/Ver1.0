using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<OrderRecord> _orderRepository;
        private readonly IRepository<LinkOrderCampaignProductRecord> _ocpRepository;
        private readonly IRepository<CurrencyRecord> _currencyRepository;
        private readonly IRepository<ProductSizeRecord> _sizeRepository;
        private readonly IRepository<OrderStatusRecord> _orderStatusRepository;
        private readonly ICampaignService _campaignService;

        public OrderService(IRepository<OrderRecord> orderRepository, 
                            IRepository<LinkOrderCampaignProductRecord> ocpRepository, 
                            IRepository<CurrencyRecord> currencyRepository, 
                            ICampaignService campaignService, 
                            IRepository<ProductSizeRecord> sizeRepository,
                            IRepository<OrderStatusRecord> orderStatusRepository)
	    {
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _currencyRepository = currencyRepository;
            _campaignService = campaignService;
            _sizeRepository = sizeRepository;
            _orderStatusRepository = orderStatusRepository;
	    }

        public OrderRecord GetOrderById(int id)
        {
            return _orderRepository.Table.FirstOrDefault(r => r.Id == id);
        }

        public void UpdateOrder(OrderRecord order)
        {
            _orderRepository.Update(order);    
        }

        public OrderRecord CreateOrder(IEnumerable<OrderProductViewModel> products)
        {
            try
            {
                var order = new OrderRecord()
                {
                    Created = DateTime.UtcNow,
                    CurrencyRecord = _currencyRepository.Get(1),
                    OrderStatusRecord = _orderStatusRepository.Get(int.Parse(OrderStatus.Created.ToString("d")))
                };
                _orderRepository.Create(order);
                List<LinkOrderCampaignProductRecord> productsList = new List<LinkOrderCampaignProductRecord>();

                foreach (var product in products)
                {
                    var campaignProduct = _campaignService.GetCampaignProductById(product.ProductId);
                    var orderProduct = new LinkOrderCampaignProductRecord() { Count = product.Count, 
                                                                              ProductSizeRecord = _sizeRepository.Get(product.SizeId), 
                                                                              CampaignProductRecord = campaignProduct, 
                                                                              OrderRecord = order };
                    _ocpRepository.Create(orderProduct);
                    productsList.Add(orderProduct);
                }

                order.Products = productsList;
                return order;
            }
            catch
            {
                throw;
            }
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaigns(int[] ids)
        {
            return _ocpRepository.Table.Where(p => ids.Contains(p.CampaignProductRecord.CampaignRecord_Id));
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId)
        {
            return _ocpRepository.Table.Where(x => x.CampaignProductRecord.CampaignRecord_Id == campaignId);
        }

        public Task<int> GetProfitOfCampaign(int id)
        {
            return Task.Run<int>(() => GetProductsOrderedOfCampaign(id)
                                        .Select(p => new { Profit = p.Count * (p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost) })
                                        .Sum(entry => (int?)entry.Profit) ?? 0);
        }


        public void UpdateOrder(OrderRecord order, OrderStatus status)
        {
            order.OrderStatusRecord = _orderStatusRepository.Get(int.Parse(status.ToString("d")));
            _orderRepository.Update(order);   
        }
    }
}
