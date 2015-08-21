﻿using System.Linq;
using Orchard;
using Teeyoot.Module.Models;
using System;

namespace Teeyoot.Module.Services
{
    public interface IPromotionService : IDependency
    {
        IQueryable<PromotionRecord> GetAllPromotionsForUser(int userId);

        void DeletePromotion(int id);

        void DisablePromotion(int id);

        void ActivatePromotion(int id);

        void AddPromotion(string promoId, string discountType, double amountSize, string amountType, DateTime expiration, int UserId);

        PromotionRecord GetPromotionByPromoId(string promoId);

        PromotionRecord GetPromotion(int id);
    }

    
}
