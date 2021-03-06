﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.Services.Interfaces;
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
        private readonly IRepository<CampaignRecord> _campaignRepository;
        private readonly IOrchardServices _orchardServices;

        public OrderService(
            IRepository<OrderRecord> orderRepository,
            IRepository<LinkOrderCampaignProductRecord> ocpRepository,
            IRepository<CurrencyRecord> currencyRepository,
            ICampaignService campaignService,
            IRepository<ProductSizeRecord> sizeRepository,
            IRepository<OrderStatusRecord> orderStatusRepository,
            IRepository<OrderHistoryRecord> orderHistoryRepository,
            IRepository<ProductColorRecord> colorRepository,
            IRepository<CampaignRecord> campaignRepository,
            IOrchardServices orchardServices)
        {
            _orderRepository = orderRepository;
            _ocpRepository = ocpRepository;
            _currencyRepository = currencyRepository;
            _campaignService = campaignService;
            _sizeRepository = sizeRepository;
            _orderStatusRepository = orderStatusRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _colorRepository = colorRepository;
            _campaignRepository = campaignRepository;
            _orchardServices = orchardServices;
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
            return
                _orderRepository.Table.Where(
                    r => r.Email == email && r.IsActive && r.Created >= DateTime.Now.AddDays(-60));

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

                while (ticks%10 == 0)
                {
                    ticks = ticks/10;
                }

                order.OrderPublicId = (ticks + order.Id).ToString();
                _orderRepository.Update(order);

                var productsList = new List<LinkOrderCampaignProductRecord>();
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
                        ProductColorRecord =
                            product.ColorId == null || product.ColorId == 0
                                ? null
                                : _colorRepository.Get(product.ColorId)
                    };

                    totalPrice = totalPrice + product.Price*product.Count;


                    _ocpRepository.Create(orderProduct);
                    productsList.Add(orderProduct);
                }

                order.TotalPrice = totalPrice;
                order.Products = productsList;

                var campaignId = order.Products.First().CampaignProductRecord.CampaignRecord_Id;
                var campaign = _campaignRepository.Get(campaignId);

                // It is impossible that TeeyootUserId equals null
                if (campaign.TeeyootUserId == null)
                {
                    throw new ApplicationException(
                        "It is impossible that TeeyootUserId equals null but this finally has happened.");
                }

                var teeyootUser = _orchardServices.ContentManager.Get<TeeyootUserPart>(campaign.TeeyootUserId.Value);
                order.SellerCountry = teeyootUser.CountryRecord;

                _orderRepository.Update(order);

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
            return
                _ocpRepository.Table.Where(
                    p => p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive);
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetActiveProductsOrderedOfCampaign(int campaignId)
        {
            return
                _ocpRepository.Table.Where(
                    p =>
                        p.CampaignProductRecord.CampaignRecord_Id == campaignId && p.OrderRecord.IsActive &&
                        p.OrderRecord.OrderStatusRecord.Name != "Cancelled" &&
                        p.OrderRecord.OrderStatusRecord.Name != "Unapproved");
        }

        public IQueryable<LinkOrderCampaignProductRecord> GetAllOrderedProducts()
        {
            return _ocpRepository.Table;
        }

        public Task<int> GetProfitOfCampaign(int id)
        {
            return Task.Run<int>(() => GetProductsOrderedOfCampaign(id)
                .Select(p => new {Profit = p.Count*(p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost)})
                .Sum(entry => (int?) entry.Profit) ?? 0);
        }

        public double GetProfitActiveOrdersOfCampaign(int id)
        {
            return GetActiveProductsOrderedOfCampaign(id)
                .Select(p => new {Profit = p.Count*(p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost)})
                .Sum(entry => (double?) entry.Profit) ?? 0;
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
            campaign.ProductCountSold -= order.Products.Sum(p => (int?) p.Count) ?? 0;
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

        public bool IsOrdersForCampaignHasStatusDeliveredAndPaid(int campignId)
        {
            var allOrderForThisCampaign =
                _ocpRepository.Table.Where(
                    l =>
                        l.CampaignProductRecord.CampaignRecord_Id == campignId &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Cancelled.ToString("d")) &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Unapproved.ToString("d"))).Count();
            var allOrderForThisCampignsStatusPaidAndDelivered =
                _ocpRepository.Table.Where(
                    l =>
                        l.CampaignProductRecord.CampaignRecord_Id == campignId && l.OrderRecord.ProfitPaid == true &&
                        l.OrderRecord.OrderStatusRecord.Id == int.Parse(OrderStatus.Delivered.ToString("d"))).Count();
            var allProductSoldByOrder =
                _ocpRepository.Table.Where(
                    l =>
                        l.CampaignProductRecord.CampaignRecord_Id == campignId &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Cancelled.ToString("d")) &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Unapproved.ToString("d")))
                    .Sum(l => (int?) l.Count) ?? 0;

            if (allOrderForThisCampaign == allOrderForThisCampignsStatusPaidAndDelivered &&
                _campaignService.GetCampaignById(campignId).ProductCountSold <= allProductSoldByOrder)
            {
                return true;
            }

            return false;
        }

        public double GetProfitByCampaign(int campaignId)
        {
            return
                _ocpRepository.Table.Where(
                    l =>
                        l.CampaignProductRecord.CampaignRecord_Id == campaignId &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Cancelled.ToString("d")) &&
                        l.OrderRecord.OrderStatusRecord.Id != int.Parse(OrderStatus.Unapproved.ToString("d")))
                    .Select(
                        p => new {Profit = p.Count*(p.CampaignProductRecord.Price - p.CampaignProductRecord.BaseCost)})
                    .Sum(entry => (double?) entry.Profit) ?? 0;
        }
    }
}
