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
        IQueryable<OrderRecord> GetAllOrders();

        OrderRecord GetOrderById(int id);

        OrderRecord GetOrderByPublicId(string id);

         IQueryable<OrderRecord> GetActiveOrdersByEmailForLastTwoMoth(string email);

        OrderRecord GetActiveOrderById(int id);

        OrderRecord GetActiveOrderByPublicId(string id);

        void UpdateOrder(OrderRecord order);


        void UpdateOrder(OrderRecord order, OrderStatus status);

        OrderRecord CreateOrder(IEnumerable<OrderProductViewModel> products);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaigns(int[] ids);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId);

        IQueryable<LinkOrderCampaignProductRecord> GetAllOrderedProducts();

        IQueryable<LinkOrderCampaignProductRecord> GetActiveProductsOrderedOfCampaign(int campaignId);

        void DeleteOrder(int orderId);

        Task<int> GetProfitOfCampaign(int id);

        double GetProfitActiveOrdersOfCampaign(int id);

        bool IsOrdersForCampaignHasStatusDeliveredAndPaid(int campignId);

        double GetProfitByCampaign(int campaignId);
    }
}
