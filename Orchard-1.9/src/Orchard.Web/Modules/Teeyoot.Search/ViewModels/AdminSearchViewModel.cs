using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.ViewModels
{
    public class AdminSearchViewModel
    {
        public List<CampaignCategoriesPartRecord> CampaignCategoriesList { get; set; }

        public Action ActionId { get; set; }

        public Action Actions { get; set; }

        public enum Action
        {
            Delete,
            Check,
            Uncheck
        }
    }
}