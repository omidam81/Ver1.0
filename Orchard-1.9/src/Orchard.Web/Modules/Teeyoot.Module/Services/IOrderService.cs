using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;
using Teeyoot.Module.ViewModels;

namespace Teeyoot.Module.Services
{
    public interface IOrderService : IDependency
    {
        OrderRecord GetOrderById(int id);

        void UpdateOrder(OrderRecord order);

        OrderRecord CreateOrder(IEnumerable<OrderProductViewModel> products);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaigns(int[] ids);

        IQueryable<LinkOrderCampaignProductRecord> GetProductsOrderedOfCampaign(int campaignId);
    }
}
