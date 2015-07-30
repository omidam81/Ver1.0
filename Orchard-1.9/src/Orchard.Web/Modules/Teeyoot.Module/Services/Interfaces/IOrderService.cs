using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Common.Enums;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services
{
    public interface IOrderService : IDependency
    {
        OrderRecord GetOrderById(int id);

        OrderRecord GetOrderByPublicId(string id);

        OrderRecord GetActiveOrderById(int id);

        OrderRecord GetActiveOrderByPublicId(string id);

        void UpdateOrder(OrderRecord order);

        void UpdateOrder(OrderRecord order, OrderStatus status);

        OrderRecord CreateOrder(IEnumerable<OrderProductViewModel> products);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaigns(int[] ids);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId);

        Task<int> GetProfitOfCampaign(int id);
    }
}
