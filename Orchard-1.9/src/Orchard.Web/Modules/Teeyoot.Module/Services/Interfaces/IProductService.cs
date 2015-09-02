﻿using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teeyoot.Module.Models;

namespace Teeyoot.Module.Services
{
    public interface IProductService : IDependency
    {
        IQueryable<ProductRecord> GetAllProducts();
        IQueryable<ProductGroupRecord> GetAllProductGroups();
        IQueryable<ProductColorRecord> GetAllColorsAvailable();
        ProductRecord GetProductById(int id);
    }
}
