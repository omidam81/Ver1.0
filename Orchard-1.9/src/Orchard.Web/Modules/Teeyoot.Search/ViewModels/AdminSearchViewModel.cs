using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Search.ViewModels
{
    public class AdminSearchViewModel
    {
        public dynamic[] CampaignCategoriesList { get; set; }

        public string SearchString { get; set; }

        public string NewCategory { get; set; }

        public int ActionId { get; set; }

        public List<ActionsViewModel> Action { get; set; }


        public dynamic[] Camapigns { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public dynamic Pager { get; set; }
    }
}