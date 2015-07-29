using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface IStoreService : IDependency
    {
        IQueryable<StoreRecord> GetAllStores();

        IQueryable<StoreRecord> GetAllStoresForUser(int userId);

        StoreRecord GetStoreById(int id);

        StoreRecord GetStoreByUrl(string url);

        StoreRecord CreateStore(int? teeyootUserId, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns);

        void UpdateStore(int id, int? teeyootUserId, string title, string description, string url, bool hideStore, bool crossSelling, IList<String> selectedCampaigns);

        bool DeleteStore(int id);
    }
}
