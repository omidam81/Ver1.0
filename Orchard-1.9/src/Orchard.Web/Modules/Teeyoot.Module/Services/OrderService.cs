using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        private readonly ICampaignService _campaignService;

        public OrderService(IRepository<OrderRecord> orderRepository, 
                            IRepository<LinkOrderCampaignProductRecord> ocpRepository, 
                            IRepository<CurrencyRecord> currencyRepository, 
                            ICampaignService campaignService, 
                            IRepository<ProductSizeRecord> sizeRepository)
	    {
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _currencyRepository = currencyRepository;
            _campaignService = campaignService;
            _sizeRepository = sizeRepository;
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
                    CurrencyRecord = _currencyRepository.Get(1)
                };
                _orderRepository.Create(order);
                List<LinkOrderCampaignProductRecord> productsList = new List<LinkOrderCampaignProductRecord>();

                foreach (var product in products)
                {
                    var campaignProduct = _campaignService.GetCampaignProductById(product.ProductId);
                    var orderProduct = new LinkOrderCampaignProductRecord() { Count = product.Count, ProductSizeRecord = _sizeRepository.Get(product.SizeId), CampaignProductRecord = campaignProduct, OrderRecord = order };
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

        public IEnumerable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId)
        {
            return _ocpRepository.Table.Fetch(x => x.CampaignProductRecord.CampaignRecord_Id == campaignId);
        }

    }
}
