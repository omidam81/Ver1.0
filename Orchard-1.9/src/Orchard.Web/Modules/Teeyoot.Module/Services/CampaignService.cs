﻿using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IRepository<CampaignRecord> _repository;

        public CampaignService(IRepository<CampaignRecord> repository)
	    {
            _repository = repository;
	    }

        public IQueryable<CampaignRecord> GetAllCampaigns()
        {
            return _repository.Table;
        }

        public CampaignRecord GetCampaignByAlias(string alias)
        {
            return GetAllCampaigns().FirstOrDefault(c => c.Alias == alias);
        }
    }
}