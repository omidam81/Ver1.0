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
        private readonly IRepository<OrderHistoryRecord> _orderHistoryRepository;
        private readonly ICampaignService _campaignService;
        private readonly IRepository<ProductColorRecord> _colorRepository;

        public OrderService(IRepository<OrderRecord> orderRepository, 
                            IRepository<LinkOrderCampaignProductRecord> ocpRepository, 
                            IRepository<CurrencyRecord> currencyRepository, 
                            ICampaignService campaignService, 
                            IRepository<ProductSizeRecord> sizeRepository,
                            IRepository<OrderStatusRecord> orderStatusRepository,
                            IRepository<OrderHistoryRecord> orderHistoryRepository,
                            IRepository<ProductColorRecord> colorRepository)
	    {
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _currencyRepository = currencyRepository;
            _campaignService = campaignService;
            _sizeRepository = sizeRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _colorRepository = colorRepository;
	    }

        public IQueryable<OrderRecord> GetAllOrders()
        {
            return _orderRepository.Table;
        }

        public OrderRecord GetOrderById(int id)
        {
            return _orderRepository.Table.FirstOrDefault(r => r.Id == id);
        }

        public OrderRecord GetOrderByPublicId(string id)
        {
            return _orderRepository.Table.FirstOrDefault(r => r.OrderPublicId == id);
        }

        public IQueryable<OrderRecord> GetActiveOrdersByEmailForLastTwoMoth(string email)
        {           
            return _orderRepository.Table.Where(r => r.Email == email && r.IsActive && r.Created >= DateTime.Now.AddDays(-60));
            
        }

        public OrderRecord GetActiveOrderById(int id)
        {
            return _orderRepository.Table.FirstOrDefault(r => r.Id == id && r.IsActive);
        }

        public OrderRecord GetActiveOrderByPublicId(string id)
        {
            return _orderRepository.Table.FirstOrDefault(r => r.OrderPublicId == id && r.IsActive);
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
                    CurrencyRecord = _currencyRepository.Get(products.First().CurrencyId),
                    OrderStatusRecord = _orderStatusRepository.Get(int.Parse(OrderStatus.Approved.ToString("d"))),
                    OrderPublicId = "",
                    IsActive = false
                };

                _orderRepository.Create(order);

                var ticks = DateTime.Now.Date.Ticks;

                while(ticks % 10 == 0)
                {
                    ticks = ticks / 10;
                }

                order.OrderPublicId = (ticks + order.Id).ToString();
                _orderRepository.Update(order);

                List<LinkOrderCampaignProductRecord> productsList = new List<LinkOrderCampaignProductRecord>();
                double totalPrice = 0;
                foreach (var product in products)
                {
                    var campaignProduct = _campaignService.GetCampaignProductById(product.ProductId);
                    var orderProduct = new LinkOrderCampaignProductRecord()
                    {
                        Count = product.Count,
                        ProductSizeRecord = _sizeRepository.Get(product.SizeId),
                        CampaignProductRecord = campaignProduct,
                        OrderRecord = order,
                        ProductColorRecord = product.ColorId == null || product.ColorId == 0 ? null : _colorRepository.Get(product.ColorId)
                    };

                    totalPrice = totalPrice + product.Price * product.Count;


                    _ocpRepository.Create(orderProduct);
                    productsList.Add(orderProduct);
                }

                order.TotalPrice = totalPrice;
                _orderRepository.Update(order);
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
            return _ocpRepository.Table.Where(p => ids.Contains(p.CampaignProductRecord.CampaignRecord_Id) && p.OrderRecord.IsActive);
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId)
        {
            return _ocpRepository.Table.Where(p => p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive);
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetAllOrderedProducts()
        {
            return _ocpRepository.Table;
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

        public void DeleteOrder(int orderId)
        {
            var order = GetOrderById(orderId);

            // first, reduce product sold count of campaign
            var campaign = _campaignService.GetCampaignById(order.Products[0].CampaignProductRecord.CampaignRecord_Id);
            campaign.ProductCountSold -= order.Products.Sum(p => (int?)p.Count) ?? 0;
            _campaignService.UpdateCampaign(campaign);
            
            // second, delete products

            foreach (var p in order.Products.ToList())
            {
                _ocpRepository.Delete(p);
            }
            _ocpRepository.Flush();

            // third, delete history

            foreach (var e in order.Events.ToList())
            {
                _orderHistoryRepository.Delete(e);
            }
            _orderHistoryRepository.Flush();

            // fourth, delete order itself

            _orderRepository.Delete(order);

            _orderRepository.Flush();
        }
    }
}
