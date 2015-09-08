using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Dashboard.ViewModels
{
    public class PayoutsViewModel
    {
        public PayoutsViewModel()
        {
            Transactions = new List<History>();
            Balances = new List<Balance>();
        }

        public IList<History> Transactions { get; set; }

        public IList<Balance> Balances { get; set; }

        public dynamic[] Transacts { get; set; }

        public string filter { get; set;}
        public dynamic Pager { get; set; }


    }


    public class Balance {

        public double Bal { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }
    
    }
    
    public class History {

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Event { get; set; }

        public double Amount { get; set; }

        public bool IsPlus { get; set; }

        public int UserId { get; set; }

        public String Status { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

    }


}