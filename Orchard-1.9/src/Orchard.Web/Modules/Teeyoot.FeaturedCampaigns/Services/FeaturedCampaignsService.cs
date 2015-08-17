using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.FeaturedCampaigns.Services
{
    public class FeaturedCampaignsService : IFeaturedCampaignsService
    {
        private readonly IRepository<CampaignRecord> _campaignRepository;
        private readonly IRepository<OrderRecord> _orderRecordRepository;
        private readonly IRepository<LinkOrderCampaignProductRecord> _linkOrderCampaignProductRepository;
        private readonly IRepository<CampaignProductRecord> _campaignProductRepository;

        public FeaturedCampaignsService(IRepository<CampaignRecord> campaignRepository, IRepository<OrderRecord> orderRecordRepository, IRepository<LinkOrderCampaignProductRecord> linkOrderCampaignProductRepository, IRepository<CampaignProductRecord> campaignProductRepository)
        {
            _campaignRepository = campaignRepository;
            _orderRecordRepository = orderRecordRepository;
            _linkOrderCampaignProductRepository = linkOrderCampaignProductRepository;
            _campaignProductRepository = campaignProductRepository;
        }

        public IQueryable<CampaignRecord> GetAllCampaigns()
        {
            return _campaignRepository.Table.Where(c => c.WhenDeleted == null);
        }

        public List<OrderRecord> GetOrderForOneDay()
        {
            DateTime now = DateTime.UtcNow;
            DateTime yesterday = now.AddDays(-1);

            var ordersFromOneDay = _orderRecordRepository.Table.Where(c => c.Created <= now && c.Created >= yesterday);

            return ordersFromOneDay.ToList() ?? null;
        }

        public Dictionary<CampaignRecord, int> GetCampaignsFromOrderForOneDay(int[] ids)
        {
            var idsCampaignProductFromOneDay = _linkOrderCampaignProductRepository.Table.Where(c => ids.Contains(c.OrderRecord.Id)).Select(c => c.CampaignProductRecord.Id);
            var campaignIdsFromOneDay = _campaignProductRepository.Table.Where(c => idsCampaignProductFromOneDay.Contains(c.Id)).Select(c => c.CampaignRecord_Id);
            List<CampaignRecord> campaigns = GetAllCampaigns().Where(c => !c.IsPrivate && c.IsActive && c.IsApproved && campaignIdsFromOneDay.Contains(c.Id)).ToList();
            Dictionary<CampaignRecord, int> campSelect = new Dictionary<CampaignRecord, int>();
            foreach (var camp in campaigns)
            {
                //int k = 0;
                //for (int i = 1; i < campaigns.Count(); i++)
                //{
                //    if (campaigns.ElementAt(i).Id == camp.Id)
                //    {
                //        k++;
                //        campaigns.RemoveAt(i);
                //    }
                //}
                //campSelect.Add(camp, k);

                int count = campaigns.FindAll(x => x.Id == camp.Id).Count();
                campSelect.Add(camp, count);
            }

            campSelect.OrderByDescending(c => c.Value);
            return campSelect;
        }

        public Dictionary<CampaignRecord, int> GetCampaignsFromAdminForOneDay(List<CampaignRecord> camp)
        {
            Dictionary<CampaignRecord, int> campSelect = new Dictionary<CampaignRecord, int>();

            var orders = GetOrderForOneDay();
            if (orders != null)
            {
                int[] ids = orders.Select(c => c.Id).ToArray();
                var idsCampaignProductFromOneDay = _linkOrderCampaignProductRepository.Table.Where(c => ids.Contains(c.OrderRecord.Id)).Select(c => c.CampaignProductRecord.Id);
                var campaignIdsFromOneDay = _campaignProductRepository.Table.Where(c => idsCampaignProductFromOneDay.Contains(c.Id)).Select(c => c.CampaignRecord_Id);
                List<CampaignRecord> campaigns = GetAllCampaigns().Where(c => campaignIdsFromOneDay.Contains(c.Id)).ToList();

                foreach (var cm in camp)
                {
                    if (campaigns.Find(c => c.Id == cm.Id) == null)
                    {
                        campSelect.Add(cm, 0);
                    }
                    else
                    {
                        int count = campaigns.FindAll(x => x.Id == cm.Id).Count();
                        campSelect.Add(cm, count);
                    }
                }
            }
            else
            {
                foreach (var cm in camp)
                {
                    campSelect.Add(cm, 0);
                }
            }

            return campSelect;
        }

    }
}
