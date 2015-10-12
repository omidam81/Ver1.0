using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;
using System;

namespace Teeyoot.Module.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<PromotionRecord> _promotionRepository;
        private readonly IRepository<CurrencyRecord> _currencyRepository;

        public PromotionService(
            IRepository<PromotionRecord> promotionRepository,
            IRepository<CurrencyRecord> currencyRepository)
        {
            _promotionRepository = promotionRepository;
            _currencyRepository = currencyRepository;
        }

        public IQueryable<PromotionRecord> GetAllPromotionsForUser(int userId)
        {
            return _promotionRepository.Table.Where(x => x.UserId == userId);
        }

        public IQueryable<PromotionRecord> GetAllPromotions()
        {
            return _promotionRepository.Table;
        }

        public void DeletePromotion(int id)
        {
            _promotionRepository.Delete(_promotionRepository.Get(id));
        }

        public void DisablePromotion(int id)
        {
            var promotion = GetPromotion(id);
            promotion.Status = false;
        }

        public void ActivatePromotion(int id)
        {
            var promotion = GetPromotion(id);
            promotion.Status = true;
        }

        public void AddPromotion(
            string promoId,
            string discountType,
            double amountSize,
            string amountType,
            DateTime expiration,
            int userId,
            int? campaignId,
            DateTime created)
        {
            if (expiration == DateTime.MinValue)
            {
                expiration = DateTime.MaxValue;
            }

            var promotionCurrency = _currencyRepository.Table
                .FirstOrDefault(c => c.Code == amountType);

            var newPromotion = new PromotionRecord()
            {
                PromoId = promoId,
                DiscountType = discountType,
                AmountSize = amountSize,
                AmountType = amountType,
                Status = true,
                Expiration = expiration,
                Redeemed = 0,
                UserId = userId,
                CampaignId = campaignId,
                Created = created,
                CurrencyRecord = promotionCurrency
            };

            _promotionRepository.Create(newPromotion);
        }

        public PromotionRecord GetPromotionByPromoId(string promoId)
        {
            return _promotionRepository.Get(f => f.PromoId == promoId);
        }

        public PromotionRecord GetPromotion(int id)
        {
            return _promotionRepository.Get(f => f.Id == id);
        }

        public void CheckExpiredPromotions()
        {
            var promotions = _promotionRepository
                .Table
                .Where(c => c.Expiration < DateTime.UtcNow && c.Status)
                .ToList();

            foreach (var c in promotions)
            {
                c.Status = false;
                _promotionRepository.Update(c);
                _promotionRepository.Flush();
            }
        }
    }
}