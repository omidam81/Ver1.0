using System.Linq;
using Orchard.Data;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class SwatchService : ISwatchService
    {
        private readonly IRepository<SwatchRecord> _swatchRepository;

        public SwatchService(IRepository<SwatchRecord> swatchRepository)
        {
            _swatchRepository = swatchRepository;
        }

        public IQueryable<SwatchRecord> GetAllSwatches()
        {
            return _swatchRepository.Table;
        }
    }
}