using System.Linq;
using Orchard;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface ISwatchService : IDependency
    {
        IQueryable<SwatchRecord> GetAllSwatches();
    }
}
