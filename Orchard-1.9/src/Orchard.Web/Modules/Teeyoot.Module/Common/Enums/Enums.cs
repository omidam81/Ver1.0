using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Common.Enums
{
    public enum CampaignStatus
    {
        Active = 1,
        Ended = 2
    }

    public enum OrderStatus
    {
        Created = 1,
        Reserved,
        Printing,
        Shipped,
        Delivered
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