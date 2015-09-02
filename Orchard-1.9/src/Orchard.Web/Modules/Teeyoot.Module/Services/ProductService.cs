using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<ProductRecord> _productRepository;
        private readonly IRepository<ProductGroupRecord> _productGroupRepository;
        private readonly IRepository<ProductColorRecord> _productColorRepository;

        public ProductService(IRepository<ProductRecord> productRepository, 
                              IRepository<ProductGroupRecord> productGroupRepository,
                              IRepository<ProductColorRecord> productColorRepository)
        {
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
            _productColorRepository = productColorRepository;
        }

        public IQueryable<ProductRecord> GetAllProducts()
        {
            return _productRepository.Table.Where(c => c.WhenDeleted == null);
        }

        public IQueryable<ProductGroupRecord> GetAllProductGroups()
        {
            return _productGroupRepository.Table;
        }

        public IQueryable<ProductColorRecord> GetAllColorsAvailable()
        {
            return _productColorRepository.Table;
        }

        public ProductRecord GetProductById(int id)
        {
            return _productRepository.Get(id);
        }
    }
}