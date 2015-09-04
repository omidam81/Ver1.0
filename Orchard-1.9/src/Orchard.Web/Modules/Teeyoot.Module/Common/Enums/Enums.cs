using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Common.Enums
{
    public enum CampaignStatus
    {
        Unpaid = 1,
        //PartiallyPaid,
        Paid
    }

    public enum OrderStatus
    {
        UnApproved = 1,
        Approved,
        Printing,
        Shipped,
        Delivered,
        Paid, 
        Cancelled
    }

    public enum OverviewType
    {
        Today = 1,
        Yesterday = 2,
        Active = 4,
        AllTime = 8
    }

    public enum CampaignSortOrder
    {
        StartDate = 0,
        EndDate,
        Sales,
        Reservations,
        Name
    }
}