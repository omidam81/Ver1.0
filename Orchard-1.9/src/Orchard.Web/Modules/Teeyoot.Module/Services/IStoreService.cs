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
        IQueryable<StoreRecord> GetAllStoresForUser(int userId);

        StoreRecord GetStoreById(int id);

        StoreRecord CreateStore(StoreRecord store);

        void UpdateStore(StoreRecord store);

        bool DeleteStore(int id);
    }
}
