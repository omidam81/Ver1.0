using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using System;

namespace Teeyoot.Module.Services
{
    public interface IPromotionService : IDependency
    {
        IQueryable<PromotionRecord> GetAllPromotions();

        void DeletePromotion(int id);

        void DisablePromotion(int id);

        void ActivatePromotion(int id);

        void AddPromotion(string id, string discountType, int amountSize, string amountType, bool status, DateTime expiration);

        PromotionRecord GetPromotion(int id);
    }

    
}
