using Orchard.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Teeyoot.Module.Models
{
    public class CampaignRecord
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }

        public virtual string Alias { get; set; }

        public virtual int ProductCountGoal { get; set; }

        public virtual int ProductCountSold { get; set; }

        public virtual int ProductMinimumGoal { get; set; }

        [StringLengthMax]
        public virtual string Design { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual string URL { get; set; }

        public virtual bool IsForCharity { get; set; }

        public virtual bool BackSideByDefault { get; set; }

        public virtual int? TeeyootUserId { get; set; }

        public virtual CampaignStatusRecord CampaignStatusRecord { get; set; }

        public virtual bool IsFeatured { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsApproved { get; set; }

        public virtual bool IsArchived { get; set; }

        public virtual int? BaseCampaignId { get; set; }

        public virtual bool Rejected { get; set; }

        public virtual DateTime? WhenDeleted { get; set; }

        public virtual DateTime? WhenApproved { get; set; }

        public virtual bool IsPrivate { get; set; }

        public virtual int CntFrontColor { get; set; }

        public virtual int CntBackColor { get; set; }

        public virtual string CampaignProfit { get; set; }

        public virtual string CampaignCulture { get; set; }

        public virtual IList<CampaignProductRecord> Products { get; set; }

        public virtual IList<LinkCampaignAndCategoriesRecord> Categories { get; set; }

        public virtual IList<BringBackCampaignRecord> BackCampaign { get; set; }

        public CampaignRecord()
        {
            Products = new List<CampaignProductRecord>();
            Categories = new List<LinkCampaignAndCategoriesRecord>();
            BackCampaign = new List<BringBackCampaignRecord>();
        }
    }
}