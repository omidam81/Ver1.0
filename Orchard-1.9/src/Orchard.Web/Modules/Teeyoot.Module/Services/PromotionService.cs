using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using System;

namespace Teeyoot.Module.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<PromotionRecord> _promotionRepository;

        public PromotionService(IRepository<PromotionRecord> promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public IQueryable<PromotionRecord> GetAllPromotionsForUser(int userId)
        {
            return _promotionRepository.Table.Where(x=>x.UserId == userId);
        }

        public void DeletePromotion(int id)
        {
            _promotionRepository.Delete(_promotionRepository.Get(id));
        }

        public void DisablePromotion(int id)
        {
            PromotionRecord promotion = GetPromotion(id);
            promotion.Status = false;
        }

        public void ActivatePromotion(int id)
        {
            PromotionRecord promotion = GetPromotion(id);
            promotion.Status = true;
        }

        public void AddPromotion(string promoId, string discountType, int amountSize, string amountType,  DateTime expiration, int userId)
        {
            try
            {
                var newPromotion = new PromotionRecord()
                {
                    PromoId = promoId,
                    DiscountType = discountType,
                    AmountSize = amountSize,
                    AmountType = amountType,
                    Status = true,
                    Expiration = expiration,
                    Redeemed = 0,
                    UserId = userId
                };
                _promotionRepository.Create(newPromotion);
            }
            catch
            {
                throw;
            }
        }

        public PromotionRecord GetPromotion(int id)
        {
            return _promotionRepository.Get(f=> f.Id == id);
        }
    }
}